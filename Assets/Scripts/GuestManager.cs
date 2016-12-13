using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GuestManager : MonoBehaviour
{
    public GameObject platePS;
    public GameObject guestPrefab;
    private TableMap tables;
    private MapGenerator map;
    private Legio player;
    private Dictionary<Vector2, Guest> guestMap;
    [Range(0, 1)]
    public float baseLoad = 0.4f;
    public int bunchRange = 2;
    public float bunchSpawnDelay = 3;
    [Range(0, 1)]
    public float startSatisfaction = 0.5f;
    public float satisfyingMealSatisfaction = +0.2f;
    [Range(0, 1)]
    public float mealSatisfaction = 0.0f;
    [Range(-1, 0)]
    public float hitByWaiterSatisfaction = -0.2f;
    [Range(-1, 0)]
    public float hitByTableSatisfaction = -0.4f;
    [Range(-1, 0)]
    public float unservicedSatisfaction = -0.1f;
    [Range(-1, 0)]
    public float splatterSatisfaction = -0.8f;
    [Range(-1, 0)]
    public float missingTableSatisfaction = -0.8f;

    public float overallSatisfaction;

    public float Satisfaction
    {
        get { return overallSatisfaction; }
    }

    private static readonly Vector2[] Neighbors = {
        new Vector2(1, 0),
        new Vector2(0, 1),
        new Vector2(-1, 0),
        new Vector2(0, -1)
    };

    private bool isDay;

    // Use this for initialization
	void Start()
	{
	    tables = Objs.GetFrom<TableMap>(Objs.TABLES);
	    map = Objs.GetFrom<MapGenerator>(Objs.MAP);
	    player = Objs.GetFrom<Legio>(Objs.PLAYER);
	    guestMap = new Dictionary<Vector2, Guest>();
	    overallSatisfaction = startSatisfaction;
	    isDay = Objs.Settings().isDay;

	    InvokeRepeating("TrySpawnABunch", bunchSpawnDelay, bunchSpawnDelay);
	}

    void DayNightChanged(bool isNowDay)
    {
        isDay = isNowDay;
        if (!isNowDay)
        {
            foreach (var g in new List<Guest>(guestMap.Values))
            {
                GuestDied(g, DeathReason.NightTime);
            }
        }
    }

	// Update is called once per frame
	void Update() {
	}

    void TrySpawnABunch()
    {
        if (!Objs.Settings().isDay)
        {
            return;
        }
        SpawnABunch();
    }

    bool IsFreeFloor(Vector2 pos)
    {
        return map.IsFloorAt(pos) && player.currentPos != pos && !IsGuestAt(pos);
    }

    bool IsTableOcopied(Table t)
    {
        return FindGuestForTable(t) != null;
    }

    float DeathToSatisfaction(DeathReason reason)
    {
        switch (reason)
        {
            case DeathReason.HitByTable:
                return hitByTableSatisfaction;
            case DeathReason.HitByWaiter:
                return hitByWaiterSatisfaction;
            case DeathReason.MealConsumed:
                return mealSatisfaction;
            case DeathReason.NotServiced:
                return unservicedSatisfaction;
            case DeathReason.SatisfyingMealConsumed:
                return satisfyingMealSatisfaction;
            case DeathReason.Splattered:
                return splatterSatisfaction;
            case DeathReason.TableDisappeared:
                return missingTableSatisfaction;
            case DeathReason.NightTime:
                return 0;
            default:
                throw new Exception("Unknown death reason!");
        }
    }

    List<Pair<Vector2, Vector2>> ResolveFreePlaces(Table t)
    {
        if (IsTableOcopied(t))
        {
            return new List<Pair<Vector2, Vector2>>();
        }
        var tablePos = t.GetMapPosition();

        List<Pair<Vector2, Vector2>> possible = new List<Pair<Vector2, Vector2>>();
        foreach (var neighbor in Neighbors)
        {
            var pos = tablePos + neighbor;
            if (IsFreeFloor(pos))
            {
                possible.Add(new Pair<Vector2, Vector2>(pos, neighbor * -1));
            }
        }

        return possible;
    }

    List<Table> PlayableTables()
    {
        // find tables that are inside the restaurant area
        return tables.GetTables().Where(t => t.pos.y > 0).ToList();
    }

    List<Pair<Vector2, Vector2>> PossibleGuestLocations(List<Table> playableTables)
    {
        return playableTables.SelectMany(ResolveFreePlaces).ToList();
    }

    int SpawnNGuests(int n, List<Pair<Vector2, Vector2>> possibleGuestLocations)
    {
        int i = 0;
        for (; i < n; ++i)
        {
            if (possibleGuestLocations.Count == 0)
            {
                break;
            }
            var index = Objs.Settings().Randomness.Next(0, possibleGuestLocations.Count);
            var positions = possibleGuestLocations[index];
            SpawnGuest(positions.Left, positions.Right);
        }

        return i;
    }

    public void SpawnABunch()
    {
        var playableTables = PlayableTables();
        var possibleLocations = PossibleGuestLocations(playableTables);


        var maxiumLivingNow = (int)Mathf.Round(playableTables.Count * (baseLoad + (1 - baseLoad) * Satisfaction));
        var livingGuests = guestMap.Values.Count;


        var bunchSize = Objs.Settings().Randomness.Next(1, bunchRange);
        var maxBunchSize = maxiumLivingNow - livingGuests;
//        Debug.LogWarning(maxiumLivingNow + " " + livingGuests + " " + bunchSize + " " + maxBunchSize);
        SpawnNGuests(Math.Min(maxBunchSize, bunchSize), possibleLocations);
    }

    float Rotation(Vector2 pos)
    {
        return Mathf.Atan2(pos.x, pos.y) * Mathf.Rad2Deg;
    }

    void SpawnGuest(Vector2 at, Vector2 dir)
    {
        GameObject guestObject = Instantiate(guestPrefab);
        guestObject.transform.eulerAngles = new Vector3(0, Rotation(dir), 0);
        guestObject.transform.position = MapGenerator.ToWorldLocation(at);
        guestObject.transform.Translate(Vector3.forward, Space.Self);
        Guest guest = guestObject.GetComponent<Guest>();
        guest.TableAt = dir;
        guest.Location = at;
        guest.desire = selectDesire();
        var plate = tables.GetTable(at + dir).GetPlate();
        if (plate)
        {
            guest.ReceivedMeal(plate.desire);
        }
        guestMap[at] = guest;
    }

    Desire selectDesire()
    {
        var desires = Enum.GetValues(typeof(Desire));
        return (Desire)desires.GetValue(Objs.Settings().Randomness.Next(desires.Length));
    }

    public Guest GetGuest(Vector2 pos)
    {
        return guestMap[pos];
    }

    public bool IsGuestAt(Vector2 pos)
    {
        return guestMap.ContainsKey(pos);
    }

    public void RemoveGuestAt(Vector2 pos)
    {
        if (IsGuestAt(pos))
        {
            var guest = GetGuest(pos);
            guestMap.Remove(pos);
            Destroy(guest.gameObject);
        }
    }

    public void GuestDied(Guest g, DeathReason reason)
    {
        RemoveGuestAt(g.Location);
        Satisfy(DeathToSatisfaction(reason));
        if (reason == DeathReason.SatisfyingMealConsumed || reason == DeathReason.MealConsumed || reason == DeathReason.NightTime)
        {
            var settings = Objs.Settings();
            settings.AddMoney(settings.mealSellPrice);
            CleanupTable(g);
        }
    }

    public void CleanupTable(Guest g)
    {
        var table = tables.GetTable(g.Location + g.TableAt);
        var plate = table.gameObject.GetComponentInChildren<Plate>();
        if (plate)
        {
            Instantiate(platePS, plate.gameObject.transform.position, platePS.transform.rotation);
            Destroy(plate.gameObject);
        }
    }

    public void Satisfy(float satisfaction)
    {
        overallSatisfaction = Mathf.Min(Mathf.Max(overallSatisfaction + satisfaction, 0), 1);
    }

    public Guest FindGuestForTable(Table t)
    {
        var occupyingGuests = guestMap.Values.Where(guest => (guest.Location + guest.TableAt).Equals(t.pos)).ToArray();
        // there should only ever be one guest occupying a table!
        if (occupyingGuests.Length != 1)
        {
            return null;
        }
        return occupyingGuests.First();
    }

    public class Pair<L, R>
    {
        public readonly L Left;
        public readonly R Right;

        public Pair(L left, R right)
        {
            Left = left;
            Right = right;
        }

        public override string ToString()
        {
            return "(" + Left + ", " + Right + ")";
        }
    }
}

public enum Desire
{
    Pizza,
    Soup,
    Veggie,
    Meat,
    Alien
}

public enum DeathReason
{
    SatisfyingMealConsumed,
    MealConsumed,
    TableDisappeared,
    HitByWaiter,
    HitByTable,
    Splattered,
    NotServiced,
    NightTime
}