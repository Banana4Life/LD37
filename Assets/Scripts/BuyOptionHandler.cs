using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyOptionHandler : MonoBehaviour
{

    public List<BuyableItem> BuyableItems;

	// Use this for initialization
	void Start () {
	    foreach (var buyableItem in BuyableItems)
	    {
	        buyableItem.Init();
	    }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
