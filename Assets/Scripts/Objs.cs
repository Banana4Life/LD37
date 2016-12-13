using System;
using UnityEngine;

public class Objs
{
    public static readonly string MAP = "Map";
    public static readonly string TILES = "Tiles";
    public static readonly string TABLES = "Tables";
    public static readonly string GUESTS = "Guests";
    public static readonly string PLAYER = "Legio";
    public static readonly string SETTINGS = "Settings";
    public static readonly string SOUND = "Sound";
    public static readonly string MENUHANDLER = "MainCanvas";

    public static GameObject Get(string name)
    {
        return GameObject.Find(name);
    }

    public static T GetFrom<T>(string name)
    {
        T c = Get(name).GetComponent<T>();
        if (c == null)
        {
            throw new Exception("Component not available! Wrong object?");
        }
        return c;
    }

    public static Settings Settings()
    {
        return GetFrom<Settings>(SETTINGS);
    }

    public static Orchestrator Orchestrator()
    {
        return GetFrom<Orchestrator>(SOUND);
    }

    public static void Broadcast(String message, object value)
    {
        foreach (var go in UnityEngine.Object.FindObjectsOfType<GameObject>())
        {
            if (go.transform.parent == null)
            {
                go.BroadcastMessage(message, value, SendMessageOptions.DontRequireReceiver);
            }
        }
    }

}
