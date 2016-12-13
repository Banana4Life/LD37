using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plate : MonoBehaviour
{
    public List<Mesh> meshes;
    public Desire desire;
    public bool empty = false;

    // Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (empty)
	    {
	        gameObject.GetComponent<MeshFilter>().mesh = meshes[0];
	    }
	    else
	    {
	        gameObject.GetComponent<MeshFilter>().mesh = meshes[(int) desire + 1];
	    }

	}
}
