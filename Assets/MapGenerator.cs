using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject floorTile;
    public GameObject wallTile;
    public GameObject player;
    public GameObject mapObject;
    public GameObject tilesObject;

    public int width;
    public int height;

    public String seed;
    private long longSeed;

    private Dictionary<Vector2, GameObject> map;


    // Use this for initialization
    void Start()
    {
        longSeed = seed.GetHashCode();
        GenerateMap();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void GenerateMap()
    {
        map = new Dictionary<Vector2, GameObject>();
        var center = new Vector2(0, 0);
        for (int x = -width / 2; x < width / 2; x++)
        {
            for (int y = -height / 2; y < height / 2; y++)
            {
                if (y > 0)
                {
                    var loc = new Vector2(x, y);
                    map[loc] = Instantiate(floorTile, new Vector3(x, 0, y) * 4.5f - new Vector3(0, 6.5f, 0), floorTile.transform.rotation);
                    map[loc].transform.parent = tilesObject.transform;

                    if (!((center - loc).sqrMagnitude < 8 * 8))
                    {
                        var wall = Instantiate(wallTile, new Vector3(x, 0, y) * 4.5f, wallTile.transform.rotation);
                        wall.transform.parent = map[loc].transform;
                    }

                }

            }
        }
    }

    interface TileInfo
    {
    }

    public class Floor : TileInfo
    {
    }

    public class Table : TileInfo
    {
    }
}