using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FoodChooserMenuItem : UIListItem {
    private bool _highlighted;
    public Image BackgorundImage;
    public GameObject Pointer;

    public Color HighlightedColor;
    public Color NormalColor;

    public Desire Desire;

    public UnityEvent OnSubmit;

    // Use this for initialization
	void Start ()
	{
	    BackgorundImage.color = _highlighted ? HighlightedColor : NormalColor;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override bool Highlighted
    {
        get { return _highlighted; }
        set
        {
            BackgorundImage.color = value ? HighlightedColor : NormalColor;
            _highlighted = value;
            Objs.GetFrom<Legio>(Objs.PLAYER).selectedFood = Desire;
            Pointer.SetActive(_highlighted);
        }
    }
}
