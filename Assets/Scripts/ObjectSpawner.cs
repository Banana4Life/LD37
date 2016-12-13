using System;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    const float OPENING_SPEED = 5;

    public GameObject door;

    private bool doorOpen;
    private BuyableItem item;
    private bool doCloseDoors;
    private bool doMoveTableThroughDoors;
    private float openingAngle;

    public void spawnItem(BuyableItem newItem)
    {

		var map = Objs.Get (Objs.MAP);
		for (int i = -1; i < 2; i++) {
			for (int j = -2; j > -5; j--) {
				if (!map.GetComponent<MapGenerator>().map [new Vector2(i, j)].GetComponent<TileInfo>().canPass()) {
					return;
				}
			}
		}

        var settings = Objs.Settings();
        settings.RemoveMoney(newItem.Costs);
        Orchestrator.play_delivery();

        if (item == null) {
			item = newItem;
		}
	}

	void Update() {
	    if (item != null)
	    {
	        if (!doorOpen && item.openDoors)
	        {
	            rotDoor(true);
	        }
	        else
	        {
	            item.SpawnItem();
	            item = null;
	            doMoveTableThroughDoors = true;
	        }
	    }
	    else if (doMoveTableThroughDoors)
	    {
	        //TODO: animate
	        doMoveTableThroughDoors = false;
	        doCloseDoors = true;
	    }
	    else if (doCloseDoors)
	    {
	        rotDoor(false);
	    }
	}

    void rotDoor(bool open)
    {
        Transform[] transforms = door.GetComponentsInChildren<Transform>();
        float deltaAngle = open ? OPENING_SPEED : -OPENING_SPEED;
        if (open && openingAngle + OPENING_SPEED > 90)
        {
            deltaAngle = OPENING_SPEED - openingAngle + deltaAngle - 90;
        }
        else if (!open && openingAngle + OPENING_SPEED < 0)
        {
            deltaAngle = OPENING_SPEED - openingAngle + deltaAngle;
        }
        transforms[1].Rotate(new Vector3(0, deltaAngle, 0));
        transforms[2].Rotate(new Vector3(0, -deltaAngle, 0));
        openingAngle += deltaAngle;
        if (open && openingAngle >= 90)
        {
            doorOpen = true;

        }
        else if (!open && openingAngle <= 0)
        {
            doorOpen = false;
            doCloseDoors = false;
        }
    }
}
