using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Random = System.Random;

public class Settings : MonoBehaviour
{
    public bool isDay = true;
    public bool paused;
    public bool freezePlayer;

    public Random Randomness { get; private set; }
    public bool fixedSeed;
    public int seed = 1;

    [Range(10,150)]
    public int DayNightDuration;

    public GameObject StoryScreen;
    public MenuHandler MenuHandler;

    [Header("HUD")]
    public Text timeInfo;
    public Text dayInfo;
    public Image dayCycleImage;
    public Sprite daySprite;
    public Sprite nightSprite;
    public Text moneyText;

    public GameMode GameMode { get; set; }

    private Stopwatch currentDayTime;
    private const string CHANGEDAYTIME_METHOD = "ChangeDayTime";

    [Header("Money")]
    public int Money;
    public bool showMoneyAsRoman;
    public int rent = 20;
    public int mealMakePrice = 1;
    public int mealSellPrice = 6;

    private int dayCount;


    void Awake()
    {
        if (!fixedSeed)
        {
            seed = Environment.TickCount;
        }
        Randomness = new Random(seed);
    }


    // Use this for initialization
	void Start ()
	{
	    if (GameControl.Instance == null)
	        Debug.LogWarning("Gamemode couldn't be determinded, setting it to story mode. Ignore this if you started the game in the main scene.");
	    else
	        GameMode = GameControl.Instance.GameMode;

	    currentDayTime = new Stopwatch();
	    currentDayTime.Start();

	    ChangeDayTime();
	    InvokeRepeating(CHANGEDAYTIME_METHOD, DayNightDuration, DayNightDuration);
	    if (GameMode == GameMode.STORYMODE)
	        StartStoryMode();
	}
	
	// Update is called once per frame
	void Update ()
	{
	    var time = GetRemainingTime();
	    if (currentDayTime != null)
	        timeInfo.text = String.Format("{0}:{1}.{2}", time.Minutes, time.Seconds, time.Milliseconds / 100);

        moneyText.text = showMoneyAsRoman ? Money.ConvertToRoman() : Money.ToString();
	}

    public void AddMoney(int amount)
    {
        Money += amount;
        Objs.Broadcast("MoneyChanged", amount);
    }

    public bool RemoveMoney(int amount)
    {
        if (Money >= amount)
        {
            Money -= amount;
            Objs.Broadcast("MoneyChanged", -amount);
            return true;
        }

        return false;
    }

    private void StartStoryMode()
    {
        StoryScreen.SetActive(true);
        MenuHandler.currentMenu = StoryScreen;
        StopTime();
    }

    public Boolean CanPlayerMove()
    {
        return paused || freezePlayer;
    }

    #region time management

    public void StopTime()
    {
        paused = true;
        CancelInvoke(CHANGEDAYTIME_METHOD);
        currentDayTime.Stop();
        Objs.Orchestrator().PauseMusic();
    }

    public void ContinueTime()
    {
        paused = false;
        InvokeRepeating(CHANGEDAYTIME_METHOD, (float)GetRemainingTime().TotalSeconds, DayNightDuration);
        currentDayTime.Start();
        Objs.Orchestrator().ResumeMusic();
    }

    public void ChangeDayTime()
    {
        isDay ^= true;
        if (isDay)
            Orchestrator.play_day_start();
        else
            Orchestrator.play_day_end();

        Objs.Broadcast("DayNightChanged", isDay);

        if (isDay)
        {
            RenderSettings.ambientSkyColor = new Color(1.0f, 1.0f, 1.0f);
            RenderSettings.ambientEquatorColor = new Color(0.5661765f, 0.5610085f, 0.4912413f);
            dayCycleImage.sprite = daySprite;
            Objs.Orchestrator().SwitchToDay();
        }
        else
        {
            if (dayCount > 1) {
                if (!RemoveMoney(rent))
                {
                    Objs.Broadcast("GameLost", dayCount);
                }
            }
            RenderSettings.ambientSkyColor = new Color(0.663f, 0.678f, 1f);
            RenderSettings.ambientEquatorColor = new Color(0.325f, 0.341f, 0.655f);
            dayCycleImage.sprite = nightSprite;
            Objs.Orchestrator().SwitchToNight();
            dayCount++;
        }

        UpdateDayText();

        currentDayTime.Reset();
        currentDayTime.Start();
        // TODO change soundtrack
        // TODO change daylight
    }

    /// <summary>
    /// Returns the time that is left until the daytime changes. This is not the time until the game ends.
    /// </summary>
    /// <returns></returns>
    public TimeSpan GetRemainingTime()
    {
        TimeSpan time = TimeSpan.FromSeconds(DayNightDuration) - currentDayTime.Elapsed;
        return time >= TimeSpan.Zero ? time : TimeSpan.Zero;
    }

    private void UpdateDayText()
    {
        dayInfo.text = String.Format("{0} {1}", isDay ? "Day" : "Night", dayCount.ConvertToRoman());
    }

    #endregion
}

public enum GameMode
{
    SANDBOX,
    STORYMODE
}
