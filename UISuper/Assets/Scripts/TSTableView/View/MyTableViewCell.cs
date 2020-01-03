using System;
using System.Collections;
using System.Collections.Generic;
using Tacticsoft;
using UnityEngine;
using UnityEngine.UI;

public class MyTableViewCell : TableViewCell
{
    public Slider slider;
    public Action<MyTableViewCell, MyCellInfo> cellCallback;
    private MyCellInfo info;

    // Start is called before the first frame update
    void Start()
    {
        slider.onValueChanged.AddListener(onSliderValueChanged);
    }

    public void updateInfo(MyCellInfo model)
    {
        info = model;
        slider.value = model.height;
    }
    void onSliderValueChanged(float value)
    {
        info.height = value;
        cellCallback(this, info);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

public class MyCellInfo
{
    public int index;
    public float height;
    public string name;
}