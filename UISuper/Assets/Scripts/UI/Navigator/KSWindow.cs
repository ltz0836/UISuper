using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSWindow : MonoBehaviour
{
    public KSNavigatorBar navigator_bar;

    public static T CreateWindow<T>() where T : KSWindow
    {
        string prefab_psth = KSExtension.GetPrefabPath<T>();
        GameObject prefab_go = (GameObject)Resources.Load(prefab_psth);
        prefab_go = Instantiate(prefab_go);
        prefab_go.name = typeof(T).Name;
        T prefab_component = prefab_go.GetComponent<T>();
        return prefab_component;
    }
}
