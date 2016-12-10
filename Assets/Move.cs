using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    private bool moving = false;
    public GameObject currentTile;
    private Vector2 currentPos;
    private GameObject player;
    public GameObject map;

    private int animationFrame = 0;
    private MapGenerator mapGen;

    // Use this for initialization
    void Start()
    {
        player = gameObject;
        mapGen = map.GetComponent<MapGenerator>();
        currentPos = new Vector2(0,0);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTile == null)
        {
            currentTile = mapGen.getMap()[new Vector2(0,0)];
        }
        animationFrame++;
        if (!moving && currentTile != null && animationFrame % 20 == 0)
        {
            player.transform.position = currentTile.transform.position + Vector3.up * player.transform.position.y;
        }

        var body = gameObject.GetComponent<Rigidbody>();
        if (!moving)
        {
            var x = Input.GetAxis("Vertical");
            var y = Input.GetAxis("Horizontal");
            if (x != 0 || y != 0)
            {
                // TODO Check if Move X - Y is possible
                var move = new Vector2(x, x != 0 ? 0 : y);
                Debug.Log(move);
                var newTile = mapGen.getMap()[currentPos + move.normalized];
                if (newTile != null)
                {
                    currentTile = newTile;
                }
                moving = true;
            }
        }
        else
        {
            // TODO animation
        }

    }
}
