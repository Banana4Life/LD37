﻿using UnityEngine;

public class PSAutoDestroy : MonoBehaviour
{
    private ParticleSystem ps;

	void Start ()
	{
	    ps = GetComponent<ParticleSystem>();
	}

	void Update () {
	    if (ps && !ps.IsAlive())
	    {
	        Destroy(gameObject);
	    }
	}
}
