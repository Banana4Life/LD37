using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orchestrator : MonoBehaviour
{

    public AudioSource maintheme_night;
    public AudioSource maintheme_day;
    public AudioSource maintheme_day_long;
    private AudioSource activeTrack;
    private AudioSource inactiveTrack;
    public float musicFadeOutSpeed = 2;
    public float musicFadeInSpeed = 0.5f;

    public List<AudioSource> legionnaire_running;
    private static AudioSource legionnaire_running_chosen;
    public List<AudioSource> legionnaire_turning;
    public AudioSource legionnaire_impassable;
    public AudioSource legionnaire_tooheavy;
    public List<AudioSource> tablemove_during;
    private static AudioSource tablemove_during_chosen;
    public AudioSource tablemove_blocked;
    public List<AudioSource> microwave;
    public List<AudioSource> plate_serve;
    public List<AudioSource> plate_drop;
    public AudioSource buymenu_open;
    public AudioSource buymenu_close;
    public AudioSource buymenu_cursor;
    public AudioSource buymenu_buy;
    public AudioSource delivery;
    public AudioSource day_start;
    public AudioSource day_end;
    public List<AudioSource> guest_eating;
    public AudioSource backdrop_night;
    public AudioSource guest_spawn;
    public AudioSource bomb_beep;
    public AudioSource guest_kill;
    public AudioSource doors;
    public AudioSource buymenu_forbidden;
    public AudioSource backdrop_day;
    public AudioSource bomb_explosion;
    public List<AudioSource> guest_mad;

    private static AudioSource HandlePlay(AudioSource source)
    {
        if (source == null) return null;
        if (source.loop)
        {
            if (!source.isPlaying)
            {
                source.Play();
            }
        }
        else
        {
            source.PlayOneShot(source.clip);
        }
        return source;
    }

    private static AudioSource HandlePlay(List<AudioSource> sources)
    {
        if (sources == null) return null;
        return HandlePlay(sources[Objs.Settings().Randomness.Next(sources.Count)]);
    }

    private static void HandleStop(AudioSource source)
    {
        if (source == null) return;
        source.Stop();
    }

    public static void play_legionnaire_running()
    {
        legionnaire_running_chosen = HandlePlay(Objs.Orchestrator().legionnaire_running[0]);
    }
    public static void stop_legionnaire_running()
    {
        HandleStop(legionnaire_running_chosen);
    }
    public static void play_legionnaire_turning()
    {
        HandlePlay(Objs.Orchestrator().legionnaire_turning);
    }

    public static void play_legionnaire_impassable()
    {
        HandlePlay(Objs.Orchestrator().legionnaire_impassable);
    }

    public static void play_legionnaire_tooheavy()
    {
        HandlePlay(Objs.Orchestrator().legionnaire_tooheavy);
    }
    public static void play_tablemove_during()
    {
        tablemove_during_chosen = HandlePlay(Objs.Orchestrator().tablemove_during[0]);
    }

    public static void stop_tablemove_during()
    {
        HandleStop(tablemove_during_chosen);
    }
    public static void play_tablemove_blocked()
    {
        return; //DISABLED
        HandlePlay(Objs.Orchestrator().tablemove_blocked);
    }
    public static void play_microwave()
    {
        HandlePlay(Objs.Orchestrator().microwave);
    }
    public static void play_plate_serve()
    {
        HandlePlay(Objs.Orchestrator().plate_serve);
    }
    public static void play_plate_drop()
    {
        HandlePlay(Objs.Orchestrator().plate_drop);
    }

    public static void play_buymenu_open()
    {
        HandlePlay(Objs.Orchestrator().buymenu_open);
    }

    public static void play_buymenu_close()
    {
        HandlePlay(Objs.Orchestrator().buymenu_close);
    }
    public static void play_buymenu_cursor()
    {
        HandlePlay(Objs.Orchestrator().buymenu_cursor);
    }
    public static void play_buymenu_buy()
    {
        HandlePlay(Objs.Orchestrator().buymenu_buy);
    }
    public static void play_delivery()
    {
        HandlePlay(Objs.Orchestrator().delivery);
    }
    public static void play_day_start()
    {
        HandlePlay(Objs.Orchestrator().day_start);
    }
    public static void play_day_end()
    {
        HandlePlay(Objs.Orchestrator().day_end);
    }
    public static void play_guest_eating()
    {
        HandlePlay(Objs.Orchestrator().guest_eating);
    }
    public static void play_backdrop_night()
    {
        stop_backdrop_day();
        HandlePlay(Objs.Orchestrator().backdrop_night);
    }
    public static void stop_backdrop_night()
    {
        HandleStop(Objs.Orchestrator().backdrop_night);
    }
    public static void play_guest_spawn()
    {
        HandlePlay(Objs.Orchestrator().guest_spawn);
    }
    public static void play_bomb_beep()
    {
        HandlePlay(Objs.Orchestrator().bomb_beep);
    }
    public static void play_guest_kill()
    {
        HandlePlay(Objs.Orchestrator().guest_kill);
    }
    public static void play_doors()
    {
        HandlePlay(Objs.Orchestrator().doors);
    }
    public static void play_buymenu_forbidden()
    {
        HandlePlay(Objs.Orchestrator().buymenu_forbidden);
    }
    public static void play_backdrop_day()
    {
        stop_backdrop_night();
        HandlePlay(Objs.Orchestrator().backdrop_day);
    }
    public static void stop_backdrop_day()
    {
        HandleStop(Objs.Orchestrator().backdrop_day);
    }
    public static void play_bomb_explosion()
    {
        HandleStop(Objs.Orchestrator().bomb_beep);
        HandlePlay(Objs.Orchestrator().bomb_explosion);
    }
    public static void play_guest_mad()
    {
        HandlePlay(Objs.Orchestrator().guest_mad);
    }

    public void PauseMusic()
    {
        activeTrack.Pause();
    }

    public void ResumeMusic()
    {
        activeTrack.Play();
    }

    public void SwitchToNight()
    {
        inactiveTrack = maintheme_day;
        activeTrack = maintheme_night;
        activeTrack.Play();
        play_backdrop_night();
    }

    public void SwitchToDay()
    {
        inactiveTrack = maintheme_night;
        activeTrack = maintheme_day;
        activeTrack.Play();
        play_backdrop_day();
    }

	// Use this for initialization
	void Start ()
	{
	    activeTrack = maintheme_night;
	    inactiveTrack = maintheme_day;
	}
	
	// Update is called once per frame
	void Update () {
	    if (inactiveTrack.isPlaying) //fade out because it is not active anymore
	    {
	        inactiveTrack.volume -= Time.deltaTime * (1.0f / musicFadeOutSpeed);
	        if (inactiveTrack.volume <= 0)
	        {
	            inactiveTrack.Stop();
	        }
	    } else if (activeTrack.volume < 1) //only fade in once inactive track has faded out, hence else if
	    {
	        activeTrack.volume = Math.Min(1 , activeTrack.volume + Time.deltaTime * (1.0f / musicFadeInSpeed));
	    }
	}

}
