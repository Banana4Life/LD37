using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHandler : MonoBehaviour
{

    public GameObject mainMenu;
    public GameObject buyMenu;
    public GameObject helpScreen;
    public GameObject storyScreen;
    public GameObject gameOverScreen;
    public GameObject winScreen;

    public Vector2 computerPos = new Vector2(-1, -2);
    public Vector2 computerLookAt = new Vector2(-1, 0);

    public  GameObject currentMenu;
    private Settings settings;
    private Legio legio;
    private bool gameFinished;

	// Use this for initialization
	void Start ()
	{
	    legio = Objs.Get(Objs.PLAYER).GetComponent<Legio>();
	    settings = Objs.Settings().GetComponent<Settings>();
	}
	
    // Update is called once per frame
	void Update () {
	    // block input if game is finished
	    if (!gameFinished)
	    {
	        if (Input.GetButtonUp("Submit"))
	        {
	            if (currentMenu != null)
	                return;

	            if (isInfrontOfComputer())
	            {
	                currentMenu = buyMenu;
	                buyMenu.SetActive(true);
	                Orchestrator.play_buymenu_open();
	            }
	        }

	        if (Input.GetButtonUp("Cancel"))
	        {
	            if (currentMenu != null)
	            {
	                if (currentMenu == mainMenu || currentMenu == storyScreen || currentMenu == helpScreen)
	                    settings.ContinueTime();

	                CloseCurrentMenu();
	            }
	            else
	            {
	                // no menu present, goto main menu
	                mainMenu.GetComponent<MainMenu>().PauseGame();
	                currentMenu = mainMenu;
	            }
	        }

	        if (Input.GetButtonUp("Help"))
	        {
	            if (currentMenu == null)
	            {
	                helpScreen.SetActive(true);
	                currentMenu = helpScreen;
	                settings.StopTime();
	            }
	            else if (currentMenu == helpScreen)
	            {
	                helpScreen.SetActive(false);
	                settings.ContinueTime();
	                currentMenu = null;
	            }
	        }
	    }

	    settings.freezePlayer = currentMenu != null;
	}


    private bool isInfrontOfComputer()
    {
        // TODO non-hardcoded implementation
        // this is ugly but it works as the computer is static
        return legio.currentPos == computerPos && legio.lookAt == computerLookAt;
    }

    public void CloseCurrentMenu()
    {
        if (currentMenu == null)
            return;

        if (currentMenu == buyMenu)
            Orchestrator.play_buymenu_close();
        currentMenu.SetActive(false);
        currentMenu = null;
    }

    public void GameOver(int days)
    {

        finishGame(false);
    }

    public void Win(int days)
    {
        finishGame(true);
    }

    private void finishGame(bool win)
    {
        gameFinished = true;
        CloseCurrentMenu();
        var screen = win ? winScreen : gameOverScreen;
        screen.SetActive(true);
        currentMenu = screen;
    }
}
