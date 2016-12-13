using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FoodChooser : UIListHandler
{
    public MenuHandler MenuHandler;

    // Use this for initialization
	void Start ()
	{
	    Init();
	}
	
	// Update is called once per frame
	void Update ()
	{
        if (MenuHandler.currentMenu == null)
            DoUpdate();
	}
}
