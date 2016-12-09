using UnityEngine;

public class Look : MonoBehaviour {
    public int lookSpeed = 15;
    public Object bullet;

    // Use this for initialization
	void Start () {
	    Cursor.lockState = CursorLockMode.Locked;
	}

	// Update is called once per frame
	void Update ()
	{
	    var mov = new Vector3(transform.localEulerAngles.x - Input.GetAxis("Mouse Y"), transform.localEulerAngles.y + Input.GetAxis("Mouse X"), 0);
	    transform.localEulerAngles = mov;

	    if (Input.GetMouseButtonDown(0))
	    {
	        var gun = gameObject.transform.Find("gun").gameObject;
	        GameObject bulletInstance = (GameObject) Instantiate(bullet);
	        bulletInstance.transform.position = gun.transform.position;
	        bulletInstance.transform.rotation = gun.transform.rotation;
            bulletInstance.transform.Rotate(new Vector3(0, 1, 0), -90);
	    }
	}
}
