using System;
using System.Collections;
using System.Collections.Generic;
using Tacticsoft;
using UnityEngine;
using UnityEngine.UI;

public class EventTableviewCell : TableViewCell
{
    public Action<EventTableviewCell, MyEventInfo> callback;
    private MyEventInfo info;
    public GameObject menuPanel;
    public Button contentBtn;

    // Start is called before the first frame update
    void Start()
    {
        contentBtn.onClick.AddListener(OnButtonClick);
    }

    public void updateInfo(MyEventInfo model)
    {
        if (info != null)
        {
            if(info.isDisplay != model.isDisplay)
            {
                menuPanel.SetActive(model.isDisplay);
            }
        }
        else
        {
            menuPanel.SetActive(model.isDisplay);
        }
        info = model;
    }

    void OnButtonClick()
    {
        info.isDisplay = !info.isDisplay;

        menuPanel.SetActive(info.isDisplay);

        info.height = info.isDisplay ? 350 : 200;

        callback(this, info);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

public class MyEventInfo
{
    public int index;
    public float height;
    public string name;
    public bool isDisplay = false;
}