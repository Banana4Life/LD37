using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyableTable : BuyableItem {

    [Header("Table")]
    public bool[][] Type = TableMap.TABLE_L;

    // Use this for initialization
    void Start ()
    {

    }

    // Update is called once per frame
    void Update () {

    }

    public override void SpawnItem()
    {
        Objs.GetFrom<TableMap>(Objs.TABLES).CreateRandomTableAt(new Vector2(-1, -4));
    }
}