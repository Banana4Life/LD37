using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TileInfo : MonoBehaviour
{
    private GameObject map;
	public bool isWall = false;
    public Vector2 pos;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public bool hasMicrowave() {
		return gameObject.transform.GetComponentInChildren<Microwave> () != null;
	}

    public bool isDestructable()
    {
        return gameObject.transform.GetComponentInChildren<Undestructable>() == null;
    }

    public bool isFloor()
    {
        return !isWall;
    }

    public bool canPass()
    {
        return !isWall && !hasTable() && !hasMicrowave();
    }

    public bool hasTable()
    {
        return map.GetComponentInChildren<TableMap>().IsTableAt(pos);
    }

    public void init(GameObject map, Vector2 pos)
    {
        this.pos = pos;
        this.map = map;
    }

	public void spawn(Vector3 at, GameObject type, bool isWall = false)
    {
		var typeObject = Instantiate(type);
		typeObject.transform.position = at;
		typeObject.transform.parent = gameObject.transform;
		this.isWall = type.name == "Wall" || isWall;
    }

}
