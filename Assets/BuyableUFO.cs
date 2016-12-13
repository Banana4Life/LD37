using UnityEngine;

public class BuyableUFO : BuyableItem
{
    public GameObject ufo;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void SpawnItem()
    {
        Instantiate(ufo, ufo.transform.position, ufo.transform.rotation);
        Invoke("DestroyPlayer", 0.5f);
        Invoke("WonThis", 2.5f);
    }

    public void WonThis()
    {
        Objs.Broadcast("Win", 0);
    }

    public void DestroyPlayer()
    {
        GameObject.Find("Legio").gameObject.active = false;
    }
}
