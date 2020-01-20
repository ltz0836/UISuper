using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSNavigatorBar : MonoBehaviour
{
    public GameObject navigator_bar_go;

    public static KSNavigatorBar Init()
    {
        KSNavigatorBar navigator_bar = new KSNavigatorBar();
        navigator_bar.navigator_bar_go = (GameObject)Resources.Load("Navigator/NavigatorBar");
        return navigator_bar;
    }

    public GameObject InstantiateNavigatorBar()
    {
        GameObject navigator_bar_gob = Instantiate(navigator_bar_go);
        navigator_bar_gob.name = typeof(KSNavigatorBar).Name;
        return navigator_bar_gob;
    }
}
