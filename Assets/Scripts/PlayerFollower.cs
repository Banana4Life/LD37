using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollower : MonoBehaviour {

	public bool followPlayer;
	public float wallCameraAxis = 0f;
	public Vector3 baseRotation = new Vector3 (60f, 0f, 0f);
	public Vector3 offsetFloor = new Vector3 (0f, 4 * MapGenerator.TILE_SIZE, -(2 * MapGenerator.TILE_SIZE));
	public Vector3 offsetWall = new Vector3 (0f, 3 * MapGenerator.TILE_SIZE, -(1 * MapGenerator.TILE_SIZE));
    public Vector3 offsetEntry = new Vector3(0f, 6 * MapGenerator.TILE_SIZE, -(5 * MapGenerator.TILE_SIZE));

    // Use this for initialization
	void Start () {
		followPlayer = true;
	}

	// Update is called once per frame
	void Update () {
		var player = Objs.Get(Objs.PLAYER);
	    if (player != null)
	    {
	        var playerPos = player.transform.position;
	        var mapLoc = MapGenerator.ToMapLocation(playerPos);
	        var map = Objs.GetFrom<MapGenerator>(Objs.MAP).GetTileMap();
	        var tile = map.ContainsKey(mapLoc + new Vector2(0, -1)) ? map[mapLoc + new Vector2(0, -1)] : null;


	        var isFloor = tile == null ? false : tile.GetComponent<TileInfo>().isFloor();
	        var isEntry = playerPos.z < -2.75f;

	        setCamera(gameObject.transform, isFloor, isEntry, playerPos);
	    }
	}

	private void setCamera(Transform t, bool isFloor, bool isEntry, Vector3 playerPos)
	{
		t.position = playerPos + offsetFloor;
		t.eulerAngles = baseRotation;

		if (isEntry) {
			t.position = playerPos + offsetEntry;
			t.RotateAround (playerPos + offsetEntry, Vector3.right, wallCameraAxis);
		} else if (!isFloor) {
			t.position = playerPos + offsetWall;
			t.RotateAround (playerPos + offsetWall, Vector3.right, wallCameraAxis);
		}
	}
}
