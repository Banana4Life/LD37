using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Legio : MonoBehaviour
{
    static Animator animator;
    public GameObject map;
    private MapGenerator mapGen;

    private float lastX;
    private float lastY;

    public Vector2 currentPos;
    public Vector2 targetPos;
    public Vector2 targetMove;
    public Vector2 lookAt = new Vector2(1, 0);
    public GameObject carryObject;

    public bool debugResetToGrid = false;

    public Desire selectedFood;
    public GameObject foodCarryPrefab;


    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        mapGen = map.GetComponent<MapGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Objs.Settings().CanPlayerMove())
            return;

        var newTargetMove = getInput();
        var newTargetPos = currentPos + newTargetMove;

        if (!promptMove(map, mapGen, currentPos, newTargetMove, newTargetPos, lookAt, carryObject))
        {
            targetMove = isShiftDown() ? -newTargetMove : newTargetMove;
            targetPos = currentPos;
        }
        else
        {
            targetMove = isShiftDown() ? -newTargetMove : newTargetMove;
            targetPos = currentPos + targetMove;
        }

        if (targetMove == Vector2.zero)
        {
            if (Input.GetButtonUp("Submit"))
            {
                if (carryObject)
                {
                    Debug.Log("Drop " + carryObject.name);
                    if (carryObject.name.Contains("Plate"))
                    {
                        var tile = mapGen.GetTileMap()[currentPos + lookAt];
                        if (tile.GetComponent<TileInfo>().hasTable())
                        {
                            var table = Objs.GetFrom<TableMap>(Objs.TABLES).GetTable(currentPos + lookAt);
                            if (table.GetComponentInChildren<Plate>() == null)
                            {
                                carryObject.transform.parent = table.gameObject.transform;
                                carryObject.transform.position = MapGenerator.ToWorldLocation(currentPos + lookAt) + Vector3.up * 1.85f;
                                carryObject = null;
                                Orchestrator.play_plate_serve();
                                var guest = Objs.GetFrom<GuestManager>(Objs.GUESTS).FindGuestForTable(table);
                                if (guest != null)
                                {
                                    guest.ReceivedMeal(table.GetComponentInChildren<Plate>().desire);
                                }
                            }
                        }
                    }
                    else
                    {
                        var bombMap = Objs.GetFrom<Bombs>("Bombs").bombs;
                        if (!bombMap.ContainsKey(currentPos))
                        {
                            bombMap[currentPos] = carryObject;
                            var bomber = carryObject.GetComponent<Bomber>();
                            bomber.follow = null;
                            bomber.fuseTimer = 5;
                            bomber.Activate();
                            carryObject.transform.position = gameObject.transform.position;
                            carryObject = null;
                        }
                    }

                }
                else
                {
                    // Check for Bomb
                    var bombMap = Objs.GetFrom<Bombs>("Bombs").bombs;
                    GameObject bomb = null;
                    if (bombMap.ContainsKey(currentPos))
                    {
                        bomb = bombMap[currentPos];
                        bombMap.Remove(currentPos);
                    }
                    if (bombMap.ContainsKey(currentPos + lookAt))
                    {
                        bomb = bombMap[currentPos + lookAt];
                        bombMap.Remove(currentPos + lookAt);
                    }
                    if (bomb != null)
                    {
                        Debug.Log("Pickup Bomb");
                        bomb.GetComponent<Bomber>().follow = this;
                        carryObject = bomb;
                    }

                    // Check for Microwave
                    var tile = mapGen.GetTileMap()[currentPos + lookAt].GetComponent<TileInfo>();
                    Debug.Log(tile + " " + tile.hasMicrowave());
                    if (tile.hasMicrowave())
                    {
                        var settings = Objs.Settings();
                        settings.RemoveMoney(settings.mealMakePrice);
                        Debug.Log("Pickup Plate");
                        carryObject = Instantiate(foodCarryPrefab, gameObject.transform);
                        carryObject.GetComponent<Plate>().desire = selectedFood;
                        //carryObject.transform.position = Vector3.zero;
                        carryObject.transform.localPosition = new Vector3(0, 1, 1) * 1.5f / 11f;
                        Orchestrator.play_microwave();
                    }

                    // Check for Table with Plate
                    if (tile.hasTable())
                     {
                         var table = Objs.GetFrom<TableMap>(Objs.TABLES).GetTable(currentPos + lookAt);
                         if (table.GetComponentInChildren<Plate>() != null)
                        {
                            Debug.Log("Pickup Plate");
                            carryObject = table.GetComponentInChildren<Plate>().gameObject;
                            carryObject.transform.parent = gameObject.transform;
                            carryObject.transform.localPosition = new Vector3(0, 1, 1) * 1.5f / 11f;

                            var guest = Objs.GetFrom<GuestManager>(Objs.GUESTS).FindGuestForTable(table);
                            if (guest != null)
                            {
                                guest.RemoveMeal();
                            }

                        }
                    }

                }
            }
        }

        if (debugResetToGrid)
        {
            debugResetToGrid = false;
            var mapPos = MapGenerator.ToMapLocation(animator.transform);
            animator.transform.position = MapGenerator.ToWorldLocation(mapPos);
        }
    }

    public static bool isShiftDown()
    {
        return Input.GetAxis("LShift") != 0;
    }

    private static void getAttachedTables(TableMap tables, HashSet<Table> tableGroup, Vector2 moveTo, int depth = 0)
    {
        if (depth > 10)
        {
            return;
        }

        var toAdd = new HashSet<Table>();
        foreach (var table in tableGroup)
        {
            if (tables.IsTableAt(table.pos + moveTo))
            {
                var tableObject = tables.GetTable(table.pos + moveTo);
                if (tableObject.transform.parent != table.gameObject.transform.parent) // other group
                {
                    foreach (var child in tableObject.transform.parent.GetComponentsInChildren<Table>())
                    {
                        toAdd.Add(child);
                    }
                }
                if (tableGroup.Count > 10)
                {
                    return;
                }
            }
        }
        getAttachedTables(tables, toAdd, moveTo, depth + 1);
        foreach (var table in toAdd)
        {
            tableGroup.Add(table);
        }
    }

    private static bool canPush(MapGenerator mapGen, HashSet<Table> tableGroup, Vector2 moveTo)
    {
        foreach (var table in tableGroup)
        {
            var tileInfo = mapGen.GetTileMap()[table.pos + moveTo].GetComponent<TileInfo>();
            if (!tileInfo.hasTable() && !tileInfo.canPass())
            {
                return false;
            }
        }
        return true;
    }

    public static bool promptMove(GameObject map, MapGenerator mapGen, Vector2 currentPos, Vector2 targetMove, Vector2 targetPos, Vector2 lookAt, GameObject carryObject)
    {
        //Debug.Log("Prompt L" + lookAt + " P: " + currentPos + " + " + targetMove + " => " + targetPos);
        if (mapGen.GetTileMap().ContainsKey(targetPos))
        {
            var targetTile = mapGen.GetTileMap()[targetPos];
            var tableMap = map.GetComponentInChildren<TableMap>();
            var targetTileInfo = targetTile.GetComponent<TileInfo>();
            if (targetTileInfo.canPass())
            {
                var targetPullTile = mapGen.GetTileMap()[currentPos - targetMove];
                if (carryObject == null && isShiftDown() && targetMove == -lookAt)
                {
                    if (!tableMap.IsTableAt(currentPos) && tableMap.IsTableAt(currentPos - targetMove))
                    {
                        // Pull
                        var table = tableMap.GetTable(currentPos - targetMove); // Reverse target
                        var tableGroup = new HashSet<Table>(table.transform.parent.GetComponentsInChildren<Table>().ToList());
                        getAttachedTables(tableMap, tableGroup, targetMove);
                        if (tableGroup.Count <= 10)
                        {
                            if (canPush(mapGen, tableGroup, targetMove))
                            {
                                // Debug.Log("Pull Table at " + (currentPos - targetMove) + " " + targetPos);
                                if (!tableGroup.Any(x => x.moving))
                                {
                                    foreach (var tableToMove in tableGroup)
                                    {
                                        tableToMove.startMove(targetMove, false);
                                        animator.SetBool("isPulling", true);
                                    }
                                    return true;
                                }
                            }
                            else
                            {
                                Orchestrator.play_tablemove_blocked();
                            }
                        }
                        else
                        {
                            Orchestrator.play_legionnaire_tooheavy();
                        }
                    }
                    else if (targetPullTile.GetComponent<TileInfo>().hasMicrowave())
                    {
                        //Debug.Log("Pull Microwave");
                        targetPullTile.GetComponentInChildren<Microwave>().startMove(targetMove, false);
                        animator.SetBool("isPulling", true);
                        return true;

                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            else if (carryObject == null)
            {
                if (targetTileInfo.hasTable())
                {
                    if (!isShiftDown() && targetMove == lookAt)
                    {
                        // PUSHING
                        var table = tableMap.GetTable(targetPos);
                        var tableGroup =
                            new HashSet<Table>(table.transform.parent.GetComponentsInChildren<Table>().ToList());
                        getAttachedTables(tableMap, tableGroup, targetMove);
                        if (tableGroup.Count <= 10)
                        {
                            if (canPush(mapGen, tableGroup, targetMove))
                            {
                                // Debug.Log("Push Table at " + (currentPos + targetMove) + " " + targetPos);
                                if (!tableGroup.Any(x => x.moving))
                                {
                                    foreach (var tableToMove in tableGroup)
                                    {
                                        tableToMove.startMove(targetMove, true);
                                        animator.SetBool("isPushing", true);
                                    }
                                    return true;
                                }
                            }
                            else
                            {
                                Orchestrator.play_tablemove_blocked();
                            }
                        }
                        else
                        {
                            Orchestrator.play_legionnaire_tooheavy();
                        }
                    }

                }
                else if (targetTileInfo.hasMicrowave() && !isShiftDown() && targetMove == lookAt)
                {
                    //Debug.Log("Push Microwave");
                    var behindTargetTile = mapGen.GetTileMap()[targetPos + lookAt];
                    if (behindTargetTile.GetComponent<TileInfo>().canPass())
                    {
                        animator.SetBool("isPushing", true);
                        targetTile.GetComponentInChildren<Microwave>().startMove(targetMove, true);
                    }
                }
                // TODO other pushable objects?
            }
        }
        return false;
    }

    public static void getMoveInfo(Animator animator)
    {
        var legio = animator.gameObject.GetComponent<Legio>();
        // Debug.Log(legio.currentPos + " " + legio.targetMove + " " + legio.targetPos);

        if (legio.lookAt == legio.targetMove)
        {
            var tileMap = Objs.Get(Objs.MAP).GetComponent<MapGenerator>().GetTileMap();
            if (legio.currentPos == legio.targetPos)
            {
                animator.SetInteger("Forward", 0);
                animator.SetInteger("Left", 0);
                // Debug.Log("No Target Pos");
                return;
            }
            if (!tileMap[legio.targetPos].GetComponent<TileInfo>().canPass())
            {
                animator.SetInteger("Forward", 0);
                animator.SetInteger("Left", 0);
                // Debug.Log("Move into cannot pass"); // TODO table
                return;
            }
            animator.SetInteger("Forward", 1);
            animator.SetInteger("Left", 0);
            // Debug.Log("Move Forward");
        }
        else if (legio.targetMove == Vector2.zero)
        {
            animator.SetInteger("Forward", 0);
            animator.SetInteger("Left", 0);
        }
        else
        {
            animator.SetInteger("Forward", 0);
            var cross = Vector3.Cross(legio.lookAt, legio.targetMove);
            var angle = Vector2.Angle(legio.lookAt, legio.targetMove);
            if (cross.z < 0)
            {
                angle = -angle;
            }

            animator.SetInteger("Left", (int) angle);
            // Debug.Log("Turn Angle " + angle);
        }
    }

    private Vector2 getInput()
    {
        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");

        if (x == 0 && y == 0)
        {
            lastX = x;
            lastY = y;
            return Vector2.zero;
        }

        bool activatedX = false;
        bool activatedY = false;

        if (lastX == 0 && x != 0)
        {
            activatedX = true;
        }
        if (lastY == 0 && y != 0)
        {
            activatedY = true;
        }

        if (activatedX && activatedY)
        {
            if (Math.Abs(x) > Math.Abs(y))
            {
                activatedY = false;
            }
            else if (Math.Abs(x) < Math.Abs(y))
            {
                activatedX = false;
            }
        }
        else if (!activatedX && !activatedY)
        {
            if (y != 0 && x != 0)
            {
                if (Math.Abs(x) < Math.Abs(y))
                {
                    activatedY = true;
                }
                else if (Math.Abs(x) > Math.Abs(y))
                {
                    activatedX = true;
                }
            }
            if (y == 0)
            {
                activatedX = true;
            }
            if (x == 0)
            {
                activatedY = true;
            }
        }
        else
        {
            if (activatedX)
            {
                y = 0;
            }
            if (activatedY)
            {
                x = 0;
            }
        }

        Vector2 moveTo = targetMove;
        GameObject tile;
        if (activatedX)
        {
            moveTo = x > 0 ? Vector2.right : Vector2.left;
            if (mapGen.GetTileMap().ContainsKey(currentPos + moveTo))
            {
                tile = mapGen.GetTileMap()[currentPos + moveTo];
                if (!tile.GetComponent<TileInfo>().canPass())
                {
                    moveTo = y == 0 ? moveTo : y > 0 ? Vector2.up : Vector2.down;
                }
            }
            else
            {
                moveTo = Vector2.zero;
            }
        }
        else if (activatedY)
        {
            if (mapGen.GetTileMap().ContainsKey(currentPos + moveTo))
            {
                moveTo = y > 0 ? Vector2.up : Vector2.down;
                tile = mapGen.GetTileMap()[currentPos + moveTo];
                if (!tile.GetComponent<TileInfo>().canPass())
                {
                    moveTo = x == 0 ? moveTo : x > 0 ? Vector2.right : Vector2.left;
                }
            }
            else
            {
                moveTo = Vector2.zero;
            }
        }
        else
        {
            if (mapGen.GetTileMap().ContainsKey(currentPos + moveTo))
            {
                tile = mapGen.GetTileMap()[currentPos + moveTo];
                if (!tile.GetComponent<TileInfo>().canPass())
                {
                    moveTo = x == 0 ? Vector2.zero : x > 0 ? Vector2.right : Vector2.left;
                    tile = mapGen.GetTileMap()[currentPos + moveTo];
                    if (!tile.GetComponent<TileInfo>().canPass())
                    {
                        moveTo = y == 0 ? moveTo : y > 0 ? Vector2.up : Vector2.down;
                    }
                }
            }
            else
            {
                moveTo = Vector2.zero;
            }
        }

        lastX = x;
        lastY = y;

        return moveTo;
    }
}