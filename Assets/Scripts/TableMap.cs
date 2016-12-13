using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = System.Random;

public class TableMap : MonoBehaviour
{
    public GameObject tablePrefab;
    public GameObject innerEdgePrefab;
    public GameObject outerEdgePrefab;
    private bool init;
    public float moveTime = 0.8f;
    public float delayTime = 0.5f;

    public float moveTimePull = 0.8f;
    public float delayTimePull = 0.5f;

    private Dictionary<Vector2, Table> tables = new Dictionary<Vector2, Table>();

    public static readonly bool[][] TABLE_J = {new[] {false, true}, new[] {false, true}, new[] {true, true}};
    public static readonly bool[][] TABLE_L = {new[] {true, false}, new[] {true, false}, new[] {true, true}};
    public static readonly bool[][] TABLE_S = {new[] {true, false}, new[] {true, true}, new[] {false, true}};
    public static readonly bool[][] TABLE_Z = {new[] {false, true}, new[] {true, true}, new[] {true, false}};
    public static readonly bool[][] TABLE_T = {new[] {false, true}, new[] {true, true}, new[] {false, true}};
    public static readonly bool[][] TABLE_O = {new[] {true, true}, new[] {true, true}};

    void Start ()
	{

	}

    public T[][] Transpose<T>(T[][] input)
    {
        var height = input.Length;
        var width = 0;
        if (height > 0)
        {
            width = input[0].Length;
        }

        T[][] output = new T[width][];
        for (var y = 0; y < height; ++y)
        {
            output[y] = new T[width];
            for (var x = 0; x < width; ++x)
            {
                output[y][x] = input[x][y];
            }
        }

        return output;
    }

    void Update()
    {
        if (!init)
        {
//            CreateTableAt(TABLE_J, TableOrientation.North, new Vector2(-1, 3));
            //TableFactory.CreateTable(TABLE_L, new Vector2(0, 6), map, gameObject, tablePrefab, outerEdgePrefab, innerEdgePrefab);
            //TableFactory.CreateTable(TABLE_S, new Vector2(0, 8), map, gameObject, tablePrefab, outerEdgePrefab, innerEdgePrefab);
            //TableFactory.CreateTable(TABLE_Z, new Vector2(0, 11), map, gameObject, tablePrefab, outerEdgePrefab, innerEdgePrefab);
            //TableFactory.CreateTable(TABLE_T, new Vector2(0, 14), map, gameObject, tablePrefab, outerEdgePrefab, innerEdgePrefab);
            //TableFactory.CreateTable(TABLE_O, new Vector2(0, 4), map, gameObject, tablePrefab, outerEdgePrefab, innerEdgePrefab);
            init = true;
        }
    }

    public List<Table> CreateRandomTableAt(Vector2 at)
    {
        Random rand = Objs.Settings().Randomness;
        bool[][] shape = TABLE_J;
        var tableIndex = rand.Next(6);
        tableIndex = tableIndex == 6 ? 0 : tableIndex;
        switch (tableIndex)
        {
            case 0:
                shape = TABLE_J;
                break;
            case 1:
                shape = TABLE_L;
                break;
            case 2:
                shape = TABLE_S;
                break;
            case 3:
                shape = TABLE_Z;
                break;
            case 4:
                shape = TABLE_T;
                break;
            case 5:
                shape = TABLE_O;
                break;
        }

        var orientations = Enum.GetValues(typeof(TableOrientation));
        var orientation = (TableOrientation) orientations.GetValue(rand.Next(orientations.Length));
        return CreateTableAt(shape, orientation, at);
    }

    public List<Table> CreateTableAt(bool[][] structure, TableOrientation orientation, Vector2 at)
    {
        switch (orientation)
        {
            case TableOrientation.West:
                structure = Transpose(Transpose(Transpose(structure)));
                break;
            case TableOrientation.South:
                structure = Transpose(Transpose(structure));
                break;
            case TableOrientation.East:
                structure = Transpose(structure);
                break;
        }
        var newTables = TableFactory.CreateTable(structure, at, Objs.Get(Objs.MAP), gameObject, tablePrefab, outerEdgePrefab, innerEdgePrefab);
        foreach (var table in newTables)
        {
            AddTable(table);
        }
        return newTables;
    }

    public void AddTable(Table table)
    {
        tables[table.pos] = table;
    }

    public void RemoveTableAt(Vector2 at)
    {
        if (IsTableAt(at))
        {
            tables.Remove(at);
        }
    }

    public void DestroyTableAt(Vector2 at)
    {
        if (IsTableAt(at))
        {
            var table = GetTable(at);

            var tableGroup = table.gameObject.transform.parent;
            table.gameObject.transform.SetParent(null);
            foreach (var subTable in tableGroup.GetComponentsInChildren<Table>())
            {
                if (!((IsTableAt(subTable.pos + Vector2.up) && GetTable(subTable.pos + Vector2.up).gameObject.transform.parent == tableGroup)||
                    (IsTableAt(subTable.pos + Vector2.left) && GetTable(subTable.pos + Vector2.left).gameObject.transform.parent == tableGroup)||
                    (IsTableAt(subTable.pos + Vector2.down) && GetTable(subTable.pos + Vector2.down).gameObject.transform.parent == tableGroup)||
                    (IsTableAt(subTable.pos + Vector2.right) && GetTable(subTable.pos + Vector2.right).gameObject.transform.parent == tableGroup)))
                {
                    GameObject compoundTable = new GameObject("CompoundTable");
                    compoundTable.transform.parent = tableGroup.transform.parent;
                    subTable.transform.parent = compoundTable.transform;
                }
            }

            tables.Remove(at);
            Destroy(table.gameObject);
        }
    }

    public bool IsTableAt(Vector2 at)
    {
        return tables.ContainsKey(at);
    }

    public Table GetTable(Vector2 at)
    {
        return tables[at];
    }

    public List<Table> GetTables()
    {
        return new List<Table>(tables.Values.Select(go => go.GetComponent<Table>()));
    }
}

public enum TableOrientation
{
    North,
    East,
    South,
    West
}
