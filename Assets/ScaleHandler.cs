using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleHandler : MonoBehaviour
{
    public Transform PointerIcon;
    private GuestManager guestManager;

	// Use this for initialization
	void Start ()
	{
	    guestManager = Objs.GetFrom<GuestManager>(Objs.GUESTS);
	}
	
	// Update is called once per frame
	void Update ()
	{
	    float percentile = guestManager.Satisfaction;

	    var width = getWorldWidth(GetComponent<RectTransform>());
	    var pos = PointerIcon.transform.position;
	    var x = transform.position.x - width / 2 + percentile * width;
	    var newPos = new Vector3(x, pos.y, pos.z);
	    PointerIcon.transform.position = newPos;
	}

    private float getWorldWidth(RectTransform transform)
    {
        Vector3[] corners = new Vector3[4];
        transform.GetWorldCorners(corners);
        return Math.Abs(corners[0].x - corners[2].x);
    }
}
