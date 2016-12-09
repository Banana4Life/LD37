using UnityEngine;

public class MoveBullet : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		gameObject.transform.Translate(new Vector3(0, 0, 1));
	}
}
