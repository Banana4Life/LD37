using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Player : MonoBehaviour
{
    [Header("Position and animation")]
    public Vector2 currentPos;
    private GameObject player;
    public GameObject map;
    static Animator animator;
    public bool isMoving = false;

    private MapGenerator mapGen;

    private float lastX;
    private float lastY;

    //private String lastChristmas = "☃";

    public Vector2 moveTo;
    public Vector2 lookTo = Vector2.up;

    public float AnimationFrame;

    public Vector3 animationStartPosition;
    public Vector3 animationEndPosition;
    public Vector2 targetPos;

    public bool idle = false;

    [Header("Interaction")]
    public GameObject ComputerUI;

    // Use this for initialization
    void Start()
    {
        player = gameObject;
        mapGen = map.GetComponent<MapGenerator>();
        animator = GetComponent<Animator>();
        currentPos = new Vector2(0, 0);
        targetPos = currentPos;
    }

    public void Transition(bool fromIdle = false)
    {
        Transition(lookTo, moveTo, fromIdle);
    }

    private void Transition(Vector2 from, Vector2 to, bool fromIdle)
    {
        if (fromIdle || !isMoving)
        {
            currentPos = targetPos;
            targetPos = currentPos + to;
        }

        if (Vector2.zero != to)
        {
            isMoving = true;

            AnimationFrame = 0;
            animationStartPosition = animator.transform.position;
            animationEndPosition = getTargetTile().transform.position;

            if (!promptMove(to))
            {
                isMoving = false;
                animator.SetBool("isRunning", false);
                Debug.Log("Cannot move from " + currentPos + "to " + targetPos);
                targetPos = currentPos;
                return;
            }

            idle = false;

            if (lookTo == to)
            {
                animator.SetBool("isRerun", true);
            }

            Debug.Log("Transition " + lookTo + "->" + to + " Move " + currentPos + "->" + targetPos + " " + (fromIdle ? "fromIdle" : ""));
            if (to.magnitude != 0)
            {
                lookTo = to;
            }

            if (from == to)
            {
                animator.SetBool("isTurningAround", false);
                animator.SetBool("isTurningRight", false);
                animator.SetBool("isTurningLeft", false);
            }
            else if (from.x == -to.x || from.y == -to.y)
            {
                animator.SetBool("isTurningAround", true);
                animator.SetBool("isTurningRight", false);
                animator.SetBool("isTurningLeft", false);
                animator.SetBool("isRerun", false);
            }
            else if (false
                     || from == Vector2.up && to == Vector2.right
                     || from == Vector2.right && to == Vector2.down
                     || from == Vector2.down && to == Vector2.left
                     || from == Vector2.left && to == Vector2.up
            )
            {
                animator.SetBool("isTurningAround", false);
                animator.SetBool("isTurningRight", true);
                animator.SetBool("isTurningLeft", false);
                animator.SetBool("isRerun", false);
            }
            else
            {
                animator.SetBool("isTurningAround", false);
                animator.SetBool("isTurningRight", false);
                animator.SetBool("isTurningLeft", true);
                animator.SetBool("isRerun", false);
            }

            animator.SetBool("isRunning", true);
        }
        else
        {
            isMoving = false;
            animator.SetBool("isRunning", false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        startMove();
    }

    public GameObject getTargetTile()
    {
        var d = mapGen.GetTileMap()[targetPos];
        return d;
    }

    public bool promptMove(Vector2 moveTo)
    {
        var newTile = getTargetTile();
        if (newTile != null)
        {
            var tables = map.GetComponentInChildren<TableMap>();

            if (newTile.GetComponent<TileInfo>().canPass())
            {
                if (Input.GetAxis("LShift") != 0 && tables.IsTableAt(currentPos - moveTo))
                {
                    // PULLING
                    var table = tables.GetTable(currentPos - moveTo);
                    var tableGroup = new HashSet<Table>(table.transform.parent.GetComponentsInChildren<Table>().ToList());
                    getAttachedTables(tables, tableGroup, moveTo);
                    if (tableGroup.Count <= 10)
                    {
                        if (canPush(tableGroup, moveTo))
                        {
                            Debug.Log("Pull Table at " + (currentPos - moveTo) + " " + targetPos);
                            if (!tableGroup.Any(x => x.moving))
                            {
                                foreach (var tableToMove in tableGroup)
                                {
                                    tableToMove.startMove(moveTo, false);
                                    animator.SetBool("isPulling", true);
                                }
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (newTile.GetComponent<TileInfo>().hasTable())
                {
                    if (!isShiftDown())
                    {
                        // PUSHING
                        var table = tables.GetTable(currentPos + moveTo);
                        var tableGroup =
                            new HashSet<Table>(table.transform.parent.GetComponentsInChildren<Table>().ToList());
                        getAttachedTables(tables, tableGroup, moveTo);
                        if (tableGroup.Count <= 10)
                        {
                            if (canPush(tableGroup, moveTo))
                            {
                                Debug.Log("Push Table at " + (currentPos + moveTo)  + " " + targetPos);
                                if (!tableGroup.Any(x => x.moving))
                                {
                                    foreach (var tableToMove in tableGroup)
                                    {
                                        tableToMove.startMove(moveTo, true);
                                        animator.SetBool("isPushing", true);
                                    }
                                    return true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    this.moveTo = Vector2.zero;
                }
            }
        }
        return false;
    }

    public static bool isShiftDown()
    {
        return Input.GetAxis("LShift") != 0;
    }

    private void startMove()
    {
        var moveTo = getInput(); // Interpretation of Input
        this.moveTo = moveTo;
    }

    private bool canPush(HashSet<Table> tableGroup, Vector2 moveTo)
    {
        foreach (var table in tableGroup)
        {
            if (!mapGen.GetTileMap()[table.pos + moveTo].GetComponent<TileInfo>().isFloor())
            {
                return false;
            }
        }
        return true;
    }

    private void getAttachedTables(TableMap tables, HashSet<Table> tableGroup, Vector2 moveTo, int depth = 0)
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

        Vector2 moveTo = this.moveTo;
        GameObject tile;
        if (activatedX)
        {
            moveTo = x > 0 ? Vector2.right : Vector2.left;
            tile = mapGen.GetTileMap()[currentPos + moveTo];
            if (!tile.GetComponent<TileInfo>().canPass())
            {
                moveTo = y == 0 ? moveTo : y > 0 ? Vector2.up : Vector2.down;
            }
        }
        else if (activatedY)
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

        lastX = x;
        lastY = y;

        return moveTo;
    }
}