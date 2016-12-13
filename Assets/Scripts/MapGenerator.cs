using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class MapGenerator : MonoBehaviour
{
    public static float TILE_SIZE = 4.5f;

    public GameObject floorTile;
    public GameObject wallTile;
    public GameObject obsidianTile;
    public GameObject obsidianOverlayPlane;
    public GameObject player;
    public GameObject mapObject;
    public GameObject tilesObject;

	public GameObject obsidianSide;
	public GameObject obsidianCornerRight;
	public GameObject obsidianCornerLeft;
	public GameObject obsidianCorner;
	public GameObject obsidianCornerLittle;
	public GameObject obsidianEdge;
	public GameObject obsidianEdgeWithLittle;
	public GameObject obsidianOverlayHorizantal;
	public GameObject obsidianOverlayHorizontalFade;
	public GameObject obsidianOverlayVertical;
	public GameObject door;
	public GameObject terminal;
	public GameObject microwave;

    public int width;
    public int height;

    public int initialBlockSize = 3;
    public int initialBlockCount = 3;
    public int blockSize = 2;
    public int blockCount = 5;
    public Vector2 startLocation = new Vector2(-1, 2);

    public Dictionary<Vector2, GameObject> map;

    public Dictionary<Vector2, GameObject> GetTileMap()
    {
        return map;
    }

    // Use this for initialization
    void Start()
    {
        GenerateMap();
	}

    // Update is called once per frame
    void Update()
    {
    }

    public static Vector2 ToMapLocation(Vector3 worldLocation)
    {

        return new Vector2(Mathf.Round(worldLocation.x / TILE_SIZE), Mathf.Round(worldLocation.z / TILE_SIZE));
    }

    public static Vector2 ToMapLocation(Transform transform)
    {
        return ToMapLocation(transform.position);
    }

    public static Vector3 ToWorldLocation(Vector2 mapLocation, float y = 0)
    {
        return new Vector3(mapLocation.x * TILE_SIZE, y, mapLocation.y * TILE_SIZE);
    }

    public static Vector3 ToWorldLocation(float x, float z, float y = 0)
    {
        return new Vector3(x * TILE_SIZE, y, z * TILE_SIZE);
    }

    public bool IsFloorAt(Vector2 pos)
    {
        if (!map.ContainsKey(pos))
        {
            return false;
        }
        return map[pos].GetComponent<TileInfo>().canPass();
    }

    private GameObject CreateFloor(Vector2 loc)
    {
		if (map.ContainsKey (loc)) {
			return map [loc];
		}

        var floorObject = Instantiate(floorTile, ToWorldLocation(loc) + new Vector3(0, -0.3f, 0),
            floorTile.transform.rotation);
        floorObject.name = "Tile " + loc;
        floorObject.transform.parent = tilesObject.transform;
        map[loc] = floorObject;
        return floorObject;
    }
		
	private GameObject createObsidian(Vector2 loc, GameObject wallType, Quaternion rotate)
	{
		var obsidianObject = CreateTile(loc,wallType, rotate, true);
		var overlayObject = Instantiate(obsidianOverlayPlane, obsidianObject.transform);
		var overlayPosition = obsidianOverlayPlane.transform.position;
		overlayObject.transform.localPosition = new Vector3(overlayPosition.x, overlayPosition.y, overlayPosition.z) + new Vector3(0f, 0f, -12f);

		return obsidianObject;
	}

	private GameObject CreateFloorTile(Vector2 loc, bool wall)
    {
        return CreateTile (loc, wall ? wallTile : null);
    }

	private GameObject CreateTile(Vector2 loc, GameObject type, Quaternion rotate, bool isWall = false)
	{
		var obj = CreateTile (loc, type, isWall);
		obj.transform.rotation = rotate;
		return obj;
	}

	private GameObject CreateTile(Vector2 loc, GameObject type, bool isWall = false)
	{
		var floorObject = CreateFloor(loc);
	    var tileInfo = floorObject.GetComponent<TileInfo>();
	    if (tileInfo == null)
	    {
	        tileInfo = floorObject.AddComponent<TileInfo>();
	    }
		tileInfo.init(mapObject, loc);
		if (type != null) {	
			tileInfo.spawn (ToWorldLocation(loc), type, isWall);
		}
		return floorObject;
	}

	void SpawnAt(Vector2 pos, bool wall, bool asWall = false)
    {
		if (!map.ContainsKey(pos))
        {
            map[pos] = CreateFloorTile(pos, wall);
			map[pos].GetComponent<TileInfo> ().isWall = wall || asWall;
        }
    }

	void SpawnRect(Vector2 origin, int m, int n, bool wall, bool asWall = false)
    {
        for (int x = 0; x < m; ++x)
        {
            for (int y = 0; y < n; ++y)
            {
				SpawnAt(new Vector2(origin.x + x, origin.y - y), wall, wall || asWall);

            }
        }
    }

    void SpawnSquare(Vector2 origin, int n, bool wall)
    {
        SpawnRect(origin, n, n, wall);
    }

    void SpawnManySquares(int m, int n, bool wall, Random rand)
    {
        for (var i = 0; i < m; ++i)
        {
            List<Vector2> possibleNext = PossibleNext(n);
            if (possibleNext.Count > 0)
            {
                var next = possibleNext[rand.Next(0, possibleNext.Count)];
                SpawnSquare(next, n, wall);
            }
        }
    }

    void WrapAround(int n, bool wall, Func<GameObject, bool> tileFilter, Func<float, float, bool> placeFilter)
    {
        var actualN = n * 2 + 1;
        foreach (var v in new List<Vector2>(map.Keys))
        {
            if (tileFilter(map[v]) && placeFilter(v.x, v.y))
            {
                SpawnSquare(new Vector2(v.x - n, v.y + n), actualN, wall);
            }
        }
    }

    private bool OverlapFree(Vector2 at, int n)
    {
        for (var x = 0; x < n; ++x)
        {
            for (var y = 0; y < n; ++y)
            {
                if (map.ContainsKey(new Vector2(at.x + x, at.y - y)))
                {
                    return false;
                }
            }
        }
        return true;
    }

	private bool PlacableAt(Vector2 at, int n)
	{
		Vector2[] dirs = { Vector2.down, Vector2.left, Vector2.right };
		return OverlapFree (at, n) && dirs.Any (dir => !OverlapFree (at + dir, n));
	}

    private List<Vector2> PossibleNext(int n)
    {
        var possibleNext = new HashSet<Vector2>();
        foreach (var current in map.Keys)
        {
            if (current.y < startLocation.y)
            {
                continue;
            }
            for (var i = 0; i < n - 1; ++i)
            {
                var v = new Vector2(current.x - n, current.y - i + (n - 1));
				if (PlacableAt(v, n))
                {
                    possibleNext.Add(v);
                }
            }
            for (var i = 0; i < n + (n - 1); ++i)
            {
                var v = new Vector2(current.x + i - (n - 1), current.y + n);
				if (PlacableAt(v, n))
                {
                    possibleNext.Add(v);
                }
            }
            for (var i = 0; i < n - 1; ++i)
            {
                var v = new Vector2(current.x + n, current.y - i + (n - 1));
				if (PlacableAt(v, n))
                {
                    possibleNext.Add(v);
                }
            }
        }
        return new List<Vector2>(possibleNext);
    }

    private bool DefaultPlaceFilter(float x, float y)
    {
        return y >= startLocation.y;
    }

    private bool IsFloorFilter(GameObject go)
    {
        return go.GetComponent<TileInfo>().isFloor();
    }

    private bool IsWallFilter(GameObject go)
    {
        return !IsFloorFilter(go);
    }

    public void EnsureBorder()
    {
        Func<float, float, bool> placeFilter = (x, y) => y > 0;
        WrapAround(1, true, IsFloorFilter, placeFilter);
        for (var i = 1; i < 5; ++i)
        {
            WrapAround(1, true, IsWallFilter, placeFilter);
        }
    }

    public void DestroyAndFill(List<Vector2> locations)
    {
        foreach (var loc in locations)
        {
            if (map.ContainsKey(loc))
            {
                var tileObject = map[loc];
                if (tileObject.GetComponent<TileInfo>().isDestructable())
                {
                    DestroyDeep(tileObject);
                    map.Remove(loc);
                }
            }
            SpawnAt(loc, false);
        }
        EnsureBorder();
    }

    private void DestroyDeep(GameObject go)
    {
        if (go == null)
        {
            return;
        }
        foreach (Transform child in go.transform)
        {
            DestroyDeep(child.gameObject);
        }
        Destroy(go);
    }

    void GenerateMap()
    {
        Random rand = Objs.Settings().Randomness;
        map = new Dictionary<Vector2, GameObject>();

        SpawnSquare(startLocation, initialBlockSize, false);
//        SpawnManySquares(1, initialBlockSize, false);
        SpawnManySquares(initialBlockCount - 1, initialBlockSize, false, rand);
        SpawnManySquares(blockCount, blockSize, false, rand);
        WrapAround(1, false, IsFloorFilter, (x, y) => y > startLocation.y);
        EnsureBorder();
        EnsureObsidianWall();
        generateEntry();
    }

    // generate the floor of the entry area
    void generateEntry()
    {
		// Generate Floor 
		SpawnRect(new Vector2(-1, -1), 3, 7, false);
		// Hinter der Tür soll der Floor als Wand gewertet werden
		for (int i = -1; i < 2; i++) {
			map [new Vector2 (i, -6)].GetComponent<TileInfo> ().isWall = true;
		}
		// Mikrowelle
		CreateTile (new Vector2 (-1, 0), microwave);

		// Generate special Wall
		CreateTile (new Vector2 (-3, -1), obsidianSide, Quaternion.Euler(new Vector3(0f, 180f, 0f)), true);
		CreateTile (new Vector2 (-2, -1), obsidianCornerRight, Quaternion.Euler(new Vector3(0f, 180f, 0f)), true);
		CreateTile (new Vector2 (2, -1), obsidianCornerLeft, true);

		// Genarate Wall
		for (int i = 2; i < 8; i++) {
			switch (i) {
			case 3:
				CreateTile (new Vector2 (-2, -i), obsidianCorner, Quaternion.Euler(new Vector3(0f, -90f, 0f)), true);
				CreateTile (new Vector2 (-3, -i), obsidianSide, true);
				CreateTile (new Vector2 (-4, -i), obsidianCornerLittle, Quaternion.Euler(new Vector3(0f, -90f, 0f)), true);
				break;
			case 4:
			case 5:
				CreateTile (new Vector2 (-4, -i), obsidianSide, Quaternion.Euler (new Vector3 (0f, -90f, 0f)), true);
				if (i == 4) {
					CreateTile (new Vector2 (-3, -i), terminal, true);
				}
				SpawnRect(new Vector2(-3, -i), 2, 1, false);
				break;
			case 6:
				CreateTile (new Vector2 (-2, -i), obsidianCornerRight, Quaternion.Euler(new Vector3(0f, 180f, 0f)), true);
				CreateTile (new Vector2 (-3, -i), obsidianSide, Quaternion.Euler(new Vector3(0f, 180f, 0f)), true);
				CreateTile (new Vector2 (-4, -i), obsidianCornerLittle, Quaternion.Euler(new Vector3(0f, 180f, 0f)), true);
				break;
			default:
				CreateTile (new Vector2 (-2, -i), obsidianSide, Quaternion.Euler(new Vector3(0f, -90f, 0f)), true);
				break;
			}
		}
		for (int i = 2; i < 8; i++) {
			CreateTile (new Vector2 (2, -i), obsidianSide, Quaternion.Euler(new Vector3(0f, 90f, 0f)), true);
		}

		// Horizontales Overlay
		var overlay = Instantiate (obsidianOverlayHorizantal);
		overlay.transform.position = new Vector3 (0f, 1.75f, -8f) * MapGenerator.TILE_SIZE;
		overlay = Instantiate (obsidianOverlayHorizontalFade);
		overlay.transform.position = new Vector3 (0f, 1.75f, -6.75f) * MapGenerator.TILE_SIZE;
		// Verticales Overlay links oben
		overlay = Instantiate (obsidianOverlayVertical);
		overlay.transform.localScale = new Vector3 (1f, 0.5f, 0.5f);
		overlay.transform.position = new Vector3 (-3.5f, 1.5f, -2f) * MapGenerator.TILE_SIZE + new Vector3(0f, -0.25f, -0.35f);
		// Verticales Overlay links unten
		overlay = Instantiate (obsidianOverlayVertical);
		overlay.transform.localScale = new Vector3 (1f, 1f, 0.5f);
		overlay.transform.position = new Vector3 (-3.5f, 1.5f, -7f) * MapGenerator.TILE_SIZE + new Vector3(0.25f, 0f, 0f);
		// Verticales Overlay rechts
		overlay = Instantiate (obsidianOverlayVertical);
		overlay.transform.localScale = new Vector3 (1f, 1f, 2.5f);
		overlay.transform.position = new Vector3 (3.5f, 1.5f, -5f) * MapGenerator.TILE_SIZE + new Vector3(0f, -0.25f, 0f);
	}

    void EnsureObsidianWall()
    {
        var lowX = 0;
        for (; map.ContainsKey(new Vector2(lowX, 0)); --lowX)
        {}
        var highX = 0;
        for (; map.ContainsKey(new Vector2(highX, 0)); ++highX)
        {}

        for (int x = 3; x < highX; ++x)
        {
			createObsidian(new Vector2(x, -1), obsidianTile, new Quaternion());
		}
        for (int x = -4; x > lowX; --x)
        {
			createObsidian(new Vector2(x, -1), obsidianTile, new Quaternion());
		}
    }
}