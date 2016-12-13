using System.Collections.Generic;
using UnityEngine;

public class Bomber : MonoBehaviour
{
    public bool circle = true;
    public uint range = 2;
    public GameObject explosion;
    public Legio follow; // Player
    public float fuseTimer = 10;

    void Start()
    {
    }

    void Update()
    {
        if (follow != null)
        {
            gameObject.transform.position = follow.gameObject.transform.position +
                                   new Vector3(follow.lookAt.x, 1f, follow.lookAt.y) * 1.5f;
        }

    }

    public void Activate()
    {
        Invoke("Detonate", fuseTimer);
        Orchestrator.play_bomb_beep();
    }

    void Detonate()
    {
        Orchestrator.play_bomb_explosion();
        List<Vector2> destroy;
        var position = MapGenerator.ToMapLocation(gameObject.transform.position);
        Debug.Log("Detonate at " + position);
        if (circle)
        {
            destroy = DetonateCircle(position);
        }
        else
        {
            destroy = DetonateCross(position);
        }
        var map = Objs.GetFrom<MapGenerator>(Objs.MAP);
        var tables = Objs.GetFrom<TableMap>(Objs.TABLES);
        var guests = Objs.GetFrom<GuestManager>(Objs.GUESTS);
        map.DestroyAndFill(destroy);
        foreach (var loc in destroy)
        {
//            guests.Detonate();
            tables.DestroyTableAt(loc);
            guests.RemoveGuestAt(loc);
            Instantiate(explosion, MapGenerator.ToWorldLocation(loc), new Quaternion(1, 0, 0, 0));
        }

        var mapLoc = MapGenerator.ToMapLocation(gameObject.transform.position);
        var bombs = Objs.GetFrom<Bombs>("Bombs").bombs;
        if (bombs.ContainsKey(mapLoc))
        {
            bombs.Remove(mapLoc);
        }

        Destroy(this);
        Destroy(gameObject);
    }

    private List<Vector2> DetonateCircle(Vector2 at)
    {
        List<Vector2> locations = new List<Vector2>();
        var topLeft = at + new Vector2(-range, range);
        for (var x = 0; x < range * 2 + 1; ++x)
        {
            for (var y = 0; y < range * 2 + 1; ++y)
            {
                var destroyAt = new Vector2(topLeft.x + x, topLeft.y - y);
                locations.Add(destroyAt);
            }
        }
        return locations;
    }

    private List<Vector2> DetonateCross(Vector2 at)
    {
        List<Vector2> locations = new List<Vector2>();
        var destroyAt = at;
        for (var i = 0; i < range; ++i)
        {
            locations.Add(destroyAt + new Vector2(1, 0) * i);
            locations.Add(destroyAt + new Vector2(-1, 0) * i);
            locations.Add(destroyAt + new Vector2(0, 1) * i);
            locations.Add(destroyAt + new Vector2(0, -1) * i);
        }
        return locations;
    }
}
