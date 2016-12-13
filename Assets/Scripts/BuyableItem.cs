using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class BuyableItem : MonoBehaviour
{
    [Header("BuyableItem")]
    public int Costs;
    public string Description;
    public Sprite Icon;

    public GameObject UIPrefab;
    public UIListHandler ListHandler;

    public bool openDoors = true;

    public void Init()
    {
        var item = Instantiate(UIPrefab);
        item.transform.SetParent(ListHandler.transform);
        item.transform.localScale = Vector3.one;
        item.GetComponent<BuyableItemUI>().Init(this);
        ListHandler.ListItems.Add(item.GetComponent<UIListItem>());
    }

    public virtual void SpawnItem() {}

}
