using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public bool GamePaused;

    private Settings settings;

	// Use this for initialization
	void Start ()
	{
	    settings = Objs.Settings();
	}
	
	// Update is called once per frame
    void Update()
    {

    }

    public void PauseGame()
    {
        if (settings == null)
            settings = Objs.Settings();

        gameObject.SetActive(true);
        settings.StopTime();
    }

    public void ResumeGame()
    {
        if (settings == null)
            settings = Objs.Settings();

        settings.ContinueTime();
        gameObject.SetActive(false);
    }

    public void ToStartMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }
}
