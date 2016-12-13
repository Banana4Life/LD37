using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guest : MonoBehaviour
{
    public Vector2 TableAt;
    public Vector2 Location;
    public Desire desire;
    private bool satisfied;
    public Sprite[] desireSprites;
    public GameObject splatter;
    public float timeUntilComfortablySeated = 2;
    public float timeUntilLeaveUnserviced = 10;
    public float timeUntilEaten = 10;
    public GameObject eatingPS;

    private GameObject eatingPSInst;

    // Use this for initialization
    void Start()
    {
        Invoke("ComfortablySeated", timeUntilComfortablySeated);
        Orchestrator.play_guest_spawn();
    }

    // Update is called once per frame
    void Update()
    {
        if (!HasTable())
        {
            Die(DeathReason.TableDisappeared);
        }
    }

    public void ComfortablySeated()
    {
        Invoke("NotServiced", timeUntilLeaveUnserviced);
        var spriteRenderer = transform.Find("Desire").GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = DesireToSprite(desire);
    }

    Sprite DesireToSprite(Desire d)
    {
        return desireSprites[(int) d];
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnTriggerEnter(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
//        Debug.LogWarning("Collision!");
        var go = other.gameObject;
        if (go.GetComponent<Table>() != null || go.GetComponent<Microwave>() != null)
        {
//            Debug.LogWarning("With Table!");
            Die(DeathReason.HitByTable);
        }
        else if (go.GetComponent<Legio>() != null)
        {
//            Debug.LogWarning("With Player!");
            Die(DeathReason.HitByWaiter);
        }
    }

    public bool HasTable()
    {
        var tables = Objs.GetFrom<TableMap>(Objs.TABLES);
        return tables.IsTableAt(Location + TableAt);
    }

    private void Die(DeathReason reason)
    {
        Objs.GetFrom<GuestManager>(Objs.GUESTS).GuestDied(this, reason);

        if (reason == DeathReason.TableDisappeared
            || reason == DeathReason.HitByWaiter
            || reason == DeathReason.HitByTable
            || reason == DeathReason.NotServiced)
        {
            Orchestrator.play_guest_mad();
        }
        if (reason == DeathReason.Splattered)
        {
            Instantiate(splatter, gameObject.transform.position + splatter.transform.position, splatter.transform.rotation);
            Orchestrator.play_guest_kill();
        }
    }

    public void Splatter()
    {
        Die(DeathReason.Splattered);
    }

    void NotServiced()
    {
        Die(DeathReason.NotServiced);
    }

    void MealConsumed()
    {
        var reason = DeathReason.MealConsumed;
        if (satisfied)
        {
            reason = DeathReason.SatisfyingMealConsumed;
        }
        Die(reason);
    }

    public void RemoveMeal()
    {
        satisfied = false;
        CancelInvoke("MealConsumed");
        Invoke("NotServiced", timeUntilLeaveUnserviced / 2.0f);
        Destroy(eatingPSInst);
    }

    public void ReceivedMeal(Desire mealDesire)
    {
        CancelInvoke("NotServiced");
        satisfied = desire == mealDesire;
        Invoke("MealConsumed", timeUntilEaten);
        eatingPSInst = Instantiate(eatingPS, gameObject.transform);
        eatingPSInst.transform.localPosition = eatingPS.transform.position;
        eatingPSInst.transform.localRotation = eatingPS.transform.rotation;
        Orchestrator.play_guest_eating();
    }
}