using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[KSPrefabPath("Prefabs/Navigator/NavigatorBar")]
public class KSNavigatorBar : MonoBehaviour
{
    public Button button_back;

    public string key;

    private void Start()
    {
        button_back.onClick.AddListener(OnBackClick);
    }

    void OnBackClick()
    {
        KSNavigator.Instance.Pop(key);
    }
}
