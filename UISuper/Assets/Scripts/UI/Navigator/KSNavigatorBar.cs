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
        KSNavigator.Instance.Pop(configure);
    }

    public void UpdateConfigure(KSKitConfigure conf)
    {
        configure = new KSNavigatorBarConfigure
        {
            key = conf.key,
            camera_type = conf.camera_type
        };
    }
}

public class KSNavigatorBarConfigure
{
    public string key;
    public KSCameraType camera_type;
}