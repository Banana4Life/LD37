using UnityEngine;

public class Microwave : BuyableItem
{
    [Header("Microwave")]
    public GameObject micro;

    public Vector2 pos;
    public Vector2 targetPos;
    public TableMap tables;
    public MapGenerator map;

    public float animationFrame = 0;
    public bool moving = false;
    public Vector3 animationStart;

    // Use this for initialization
	void Start ()
	{
	    tables = Objs.GetFrom<TableMap>(Objs.TABLES);
	    map = Objs.GetFrom<MapGenerator>(Objs.MAP);
	    pos = MapGenerator.ToMapLocation(gameObject.transform);
	    targetPos = pos;
	}

    // Update is called once per frame
	void Update () {
	    animationFrame += Time.deltaTime; // time happens
	}

    public override void SpawnItem()
    {
        var spawnObject = Instantiate(micro, new Vector3(0, 0, -4 * 4.5f), micro.transform.rotation);
        spawnObject.transform.parent = Objs.GetFrom<MapGenerator>(Objs.MAP).GetTileMap()[MapGenerator.ToMapLocation(spawnObject.transform)].transform;
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
}
