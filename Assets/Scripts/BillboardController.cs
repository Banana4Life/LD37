using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	    var cam = GameObject.Find("Camera");
	    transform.LookAt(cam.transform);

	    transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward,
	        cam.transform.rotation * Vector3.up);
	}
}
