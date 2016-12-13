using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFollower : MonoBehaviour {
    [SerializeField] public Transform targetToUpdate;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        targetToUpdate.position = new Vector3(transform.position.x, targetToUpdate.position.y, transform.position.z);
	}
}
