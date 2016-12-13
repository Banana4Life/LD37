using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Table : MonoBehaviour
{
    public Vector2 pos;
    public Vector2 targetPos;
    public TableMap tables;
    public MapGenerator map;

    public float animationFrame = 0;
    public bool moving = false;
    public Vector3 animationStart;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        animationFrame += Time.deltaTime; // time happens
        updateLogicPosition();
    }

    private void updateLogicPosition()
    {
        if (tables.IsTableAt(pos) && tables.GetTable(pos).gameObject == gameObject)
        {
            tables.RemoveTableAt(pos);
        }
        //Debug.Log("Table move done " + table.pos + "->" + table.targetPos);
        pos = MapGenerator.ToMapLocation(gameObject.transform);
        tables.AddTable(this);
    }

    public bool startMove(Vector2 moveTo, bool push)
    {
        var animator = gameObject.GetComponent<Animator>();
        if (!moving)
        {
            animationStart = gameObject.transform.position;

            moving = true;
            animationFrame = 0;

            targetPos = pos + moveTo;

            if (push)
            {
                animator.SetBool("isPush", true);
            }
            else
            {
                animator.SetBool("isPull", true);
            }
        }
        return moving;
    }

    public GameObject getTargetTile()
    {
        return map.GetTileMap()[targetPos];
    }

    public Vector2 GetMapPosition()
    {
        return MapGenerator.ToMapLocation(gameObject.transform);
    }

    public List<Table> GetRelatedTables()
    {
        List<Table> related = new List<Table>();
        foreach (Transform child in gameObject.transform.parent)
        {
            related.Add(child.gameObject.GetComponent<Table>());
        }

        return related;
    }

    public Plate GetPlate()
    {
        return gameObject.GetComponentInChildren<Plate>();
    }

//    [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NotInSelectionHierarchy)]
//    static void DrawGameObjectName(Transform transform, GizmoType gizmoType)
//    {
//        var table = transform.gameObject.GetComponent<Table>();
//        if (table != null)
//        {
//            Handles.Label(transform.position, table.pos.x + ":" + table.pos.y
//                              + "\n>" + table.targetPos.x + ":" + table.targetPos.y);
//        }
//        var player = transform.gameObject.GetComponent<Legio>();
//        if (player != null)
//        {
//            Handles.Label(transform.position, player.currentPos.x + ":" + player.currentPos.y);
//            Handles.DrawWireDisc(new Vector3(player.targetPos.x, 0, player.targetPos.y) * 4.5f, Vector3.up, 1);
//        }
//    }
}