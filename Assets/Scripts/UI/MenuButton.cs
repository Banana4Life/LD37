using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButton : UIListItem
{
    public Image BackgorundImage;
    public Image ArrowLeft;
    public Image ArrowRight;

    [Range(0,1)]
    public float NormalAlpha;
    [Range(0,1)]
    public float HighlightedAlpha;

    public UnityEvent OnSubmit;

    private bool _highlighted;
    public override bool Highlighted
    {
        get { return _highlighted; }
        set
        {
            ArrowLeft.gameObject.SetActive(!Highlighted);
            ArrowRight.gameObject.SetActive(!Highlighted);
            _highlighted = value;
            ApplyAlpha();
        }
    }

    public void ApplyAlpha()
    {
        BackgorundImage.color = new Color(BackgorundImage.color.r, BackgorundImage.color.g, BackgorundImage.color.b, _highlighted ? HighlightedAlpha : NormalAlpha);
    }

    // Use this for initialization
    void Start () {
		ApplyAlpha();
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (Highlighted && OnSubmit != null && Input.GetButtonUp("Submit"))
	        OnSubmit.Invoke();
	}

    public void StartStoryMode()
    {
        GameControl.Instance.GameMode = GameMode.STORYMODE;
        SceneManager.LoadScene("Main");
    }

    public void StartSandbox()
    {
        GameControl.Instance.GameMode = GameMode.SANDBOX;
        SceneManager.LoadScene("Main");
    }

    public void Exit()
    {
        Application.Quit();
    }


}
