using System.Collections.Generic;
using UnityEngine;

public class TableFactory {
    public static List<Table> CreateTable(bool[][] shape, Vector2 basePos, GameObject mapObject, GameObject parent, GameObject prefTable, GameObject outerEdge, GameObject innerEdge)
    {
        var tableBuilder = parent.GetComponent<TableMap>();

        GameObject compoundTable = new GameObject("CompoundTable");
        compoundTable.transform.parent = parent.transform;
        var map = mapObject.GetComponent<MapGenerator>();
        compoundTable.transform.localPosition = map.GetTileMap()[basePos].transform.position;

        var tables = new List<Table>();

        for (int x = 0; x < shape.Length; x++)
        {
            for (int y = 0; y < shape[x].Length; y++)
            {
                if (shape[x][y])
                {
                    GameObject tableInst = Object.Instantiate(prefTable);
                    var table = tableInst.AddComponent<Table>();
                    tables.Add(table);
                    table.map = map;
                    table.tables = tableBuilder;
                    var delta = new Vector2(x,y);
                    var tablePos = basePos + delta;
                    table.pos = tablePos;
                    tableInst.transform.parent = compoundTable.transform;
                    tableInst.transform.position = new Vector3(tablePos.x * 4.5f, 0, tablePos.y * 4.5f);
                    if (x < shape.Length - 1 && shape[x + 1][y])
                    {
                        createTableEdge(tableInst, innerEdge, new Vector3(0, 0, 0));
                    }
                    else
                    {
                        createTableEdge(tableInst, outerEdge, new Vector3(0, 0, 0));
                    }
                    if (y > 0 && shape[x][y - 1])
                    {
                        createTableEdge(tableInst, innerEdge, new Vector3(0, 90, 0));
                    }
                    else
                    {
                        createTableEdge(tableInst, outerEdge, new Vector3(0, 90, 0));
                    }
                    if (x > 0 && shape[x - 1][y])
                    {
                        createTableEdge(tableInst, innerEdge, new Vector3(0, 180, 0));
                    }
                    else
                    {
                        createTableEdge(tableInst, outerEdge, new Vector3(0, 180, 0));
                    }
                    if (y < shape[x].Length - 1 && shape[x][y + 1])
                    {
                        createTableEdge(tableInst, innerEdge, new Vector3(0, 270, 0));
                    }
                    else
                    {
                        createTableEdge(tableInst, outerEdge, new Vector3(0, 270, 0));
                    }
                }
            }
        }

        return tables;
    }

    private static void createTableEdge(GameObject table, GameObject edge, Vector3 rotation)
    {
        GameObject innerEdgeInst = Object.Instantiate(edge);
        innerEdgeInst.transform.parent = table.transform;
        innerEdgeInst.transform.localPosition = new Vector3(0, 0, 0);
        innerEdgeInst.transform.eulerAngles = rotation;
    }
}
