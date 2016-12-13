using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to transfer data from one scene to another.
/// </summary>
public class GameControl : MonoBehaviour
{

    public static GameControl Instance;
    public GameMode GameMode;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(gameObject);
	    if (Instance == null)
	        Instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
