using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[KSPrefabPath("Prefabs/UI/Effect/Controller/UIMagicFireController")]
public class UIMagicFireController : KSWindow
{
    public Button button_next;
    // Start is called before the first frame update
    void Start()
    {
        button_next.onClick.AddListener(OnNextClick);
    }

    void OnNextClick()
    {
        KSNavigator.Instance.PushCtrl<UISettingController>(new KSKitConfigure(KSCameraType.ui, KSNavigatorBarType.nomarl));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
