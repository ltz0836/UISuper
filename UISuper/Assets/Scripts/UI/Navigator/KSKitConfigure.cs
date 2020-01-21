using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSKitConfigure
{
    public string key;
    public KSNavigatorBarType bar_type;
    public KSCameraType camera_type;
}

public enum KSNavigatorBarType
{
    nomarl,
    popup,
}

public enum KSCameraType
{
    ui,
    effect,
}