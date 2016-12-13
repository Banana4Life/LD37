using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIListHandler : MonoBehaviour
{
    public bool HasViewportAsParent;
    public List<UIListItem> ListItems;
    public UIListItem CurrentItem { get; set; }
    public bool horizontal;

    private GameObject parent;
    private int currentIndex;
    private float passedTime;
    private readonly float minIntervallSec = 0.2f;
    private bool buttonDown;
    private float lastInput;

	// Use this for initialization
	void Start ()
	{
	    Init();
	}

    public void Init()
    {
        parent = transform.parent.gameObject;
        if (ListItems.Count > 0)
        {
            CurrentItem = ListItems[0];
            CurrentItem.Highlighted = true;
        }
    }

	// Update is called once per frame
	void Update ()
	{
	    DoUpdate();
	}

    // this method exist so that it can be called from subclass
    public void DoUpdate()
    {
        passedTime += Time.deltaTime;

        if (!isActiveAndEnabled || ListItems.Count == 0)
            return;

        var axis = horizontal ? "RightLeft" : "Vertical";
        var buttonUp = Input.GetButtonUp(axis);
        if (Input.GetButtonDown(axis))
            buttonDown = true;

        var input = Input.GetAxis(axis);
        if ((lastInput >= 0 && input < 0) || (lastInput <= 0 && input > 0))
            passedTime = minIntervallSec;

        lastInput = input;

        // keyboard support and vertical controller support
        if (buttonUp)
        {
            if (passedTime >= minIntervallSec)
                HandleInput(input);

            buttonDown = false;
            return;
        }

        if (buttonDown && passedTime >= minIntervallSec)
        {
            passedTime = 0;
            HandleInput(input);
        }

        if (horizontal || buttonDown)
            return;

        // vertivcal controller support
        if (input != 0 && !Input.GetButton(axis))
        {
            if (passedTime >= minIntervallSec)
            {
                HandleInput(input);
                passedTime = 0;
            }
        }
    }

    private void HandleInput(float input)
    {
        if (input > 0)
        {
            Up();
        } else if (input < 0)
        {
            Down();
        }
    }

    public void Up()
    {
        if (currentIndex == 0)
            return;

        CurrentItem.Highlighted = false;
        currentIndex -= 1;
        CurrentItem = ListItems[currentIndex];
        CurrentItem.Highlighted = true;

        if (HasViewportAsParent)
        {
            // make the current item visible
            var parentPos = parent.transform.position;
            var pos = transform.position;
            if (parentPos.y - CurrentItem.transform.position.y < 0)
            {
                var y = pos.y + (parentPos.y - CurrentItem.transform.position.y);
                transform.position = new Vector3(pos.x, y, pos.z);
            }
        }
    }

    public void Down()
    {
        if (currentIndex == ListItems.Count - 1)
            return;

        CurrentItem.Highlighted = false;
        currentIndex += 1;
        CurrentItem = ListItems[currentIndex];
        CurrentItem.Highlighted = true;

        if (HasViewportAsParent)
        {
            // make the current item visible
            var parentPos = parent.transform.position;
            var pos = transform.position;
            var rectTransform = parent.GetComponent<RectTransform>();
            var worldHeightParent = getWorldHeight(rectTransform);
            var itemWorldHeight = getWorldHeight(CurrentItem.GetComponent<RectTransform>());

            if (worldHeightParent - (parentPos.y - CurrentItem.transform.position.y + itemWorldHeight) < 0)
            {
                var y = pos.y - worldHeightParent + parentPos.y - CurrentItem.transform.position.y + itemWorldHeight;
                transform.position = new Vector3(pos.x, y, pos.z);
            }
        }
    }

    void OnDisable()
    {
        // reset everything
        if (ListItems.Count == 0)
            return;

        while (CurrentItem != ListItems[0])
        {
            Up();
        }
    }

    private float getWorldHeight(RectTransform transform)
    {
        Vector3[] corners = new Vector3[4];
        transform.GetWorldCorners(corners);
        return Math.Abs(corners[0].y - corners[1].y);
    }
}
