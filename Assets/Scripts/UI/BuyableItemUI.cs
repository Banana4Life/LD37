using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.UI;

public class BuyableItemUI : UIListItem
{
    public Image Image;
    public Text Description;
    public Text Costs;
    public Image Background;

    private BuyableItem buyable;

    public override bool Highlighted {
        get {  return Background != null && Background.enabled; }
        set {
            if (Background != null)
            {
                Background.enabled = value;
            }
            if (value)
                Orchestrator.play_buymenu_cursor();
        }
    }

	// Use this for initialization
	void Start () {
	    if (GameControl.Instance != null && GameControl.Instance.GameMode == GameMode.SANDBOX && buyable is BuyableUFO)
	    {
	        gameObject.transform.parent.GetComponent<UIListHandler>().ListItems.RemoveAt(6);
            Destroy(gameObject);
	    }
	}
	
	// Update is called once per frame
	void Update () {
	    if (Highlighted && Input.GetButtonUp("Submit"))
	    {
	        GameObject.Find("Map").GetComponent<ObjectSpawner>().spawnItem(buyable);
	        MenuHandler menuHandler = Objs.Get(Objs.MENUHANDLER).GetComponent<MenuHandler>();
	        menuHandler.CloseCurrentMenu();
	    }
	}

    public void Init(BuyableItem item)
    {
        buyable = item;
        Description.text = item.Description;
        Image.sprite = item.Icon;
        // questionable which currency we use
        Costs.text = item.Costs.ToString();
        Background.enabled = false;
    }
}
