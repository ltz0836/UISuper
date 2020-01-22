using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[KSPrefabPath("Prefabs/UI/Settings/UISettingController")]
public class UISettingController : KSWindow
{
    public Button button_next;
    // Start is called before the first frame update
    void Start()
    {
        button_next.onClick.AddListener(OnNextClick);
    }

    void OnNextClick()
    {
        KSNavigator.Instance.PushCtrl<UIMagicFireController>(new KSKitConfigure(KSCameraType.effect, KSNavigatorBarType.nomarl));
    }
}
