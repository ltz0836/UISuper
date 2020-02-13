using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[KSPrefabPath("Prefabs/Navigator/NavigatorBar")]
public class KSNavigatorBar : MonoBehaviour
{
    public Button button_back;
    public KSNavigatorBarConfigure configure;

    private void Start()
    {
        button_back.onClick.AddListener(OnBackClick);
    }

    void OnBackClick()
    {
        KSNavigator.Instance.PopCtrl(configure);
    }

    public void UpdateConfigure(KSKitConfigure conf)
    {
        configure = new KSNavigatorBarConfigure
        {
            key = conf.key,
        };
    }
}

public class KSNavigatorBarConfigure
{
    public string key;
}