﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KSRouteController : MonoBehaviour
{
    public Button button_push;
    // Start is called before the first frame update
    void Start()
    {
        button_push.onClick.AddListener(OnPushClick);
    }

    void OnPushClick()
    {
        KSNavigator.Instance.PushCtrl<UISettingController>(new KSKitConfigure(KSNavigatorBarType.nomarl));
    }

}
