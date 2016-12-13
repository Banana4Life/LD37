using System;
using UnityEngine;

public class Bomb : BuyableItem
{
    [Header("Bomb")]
    public GameObject bomb;
    public int radius;
    public bool cycle;
    public string descriptionFormat; //This is a bomb with an explosion radius of {0} tiles. Use it to expand your area!

    // Use this for initialization
	void Awake ()
	{
	    Description = String.Format(descriptionFormat, radius);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void SpawnItem()
    {
        var bombInst = Instantiate(bomb);
        var bomber = bombInst.GetComponent<Bomber>();
        bomber.circle = cycle;
        bomber.range = (uint) radius;
		bombInst.transform.position = new Vector3(0, 0, -4 * 4.5f);
        Objs.GetFrom<Bombs>("Bombs").bombs[MapGenerator.ToMapLocation(bombInst.transform.position)] = bombInst;
    }
}
