using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    private bool moving = false;
    public GameObject currentTile;
    private GameObject player;

    // Use this for initialization
    void Start()
    {
        player = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (!moving && currentTile != null)
        {
            player.transform.position = currentTile.transform.position + Vector3.up * 5;
        }

        var body = gameObject.GetComponent<Rigidbody>();
        if (!moving)
        {
            var x = Input.GetAxis("Vertical");
            var y = Input.GetAxis("Horizontal");
            //moving = true;
        }
        else
        {
            //body.transform.position
        }

    }
}
