using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[KSPrefabPath("Prefabs/UI/Settings/UISettingController")]
public class UISettingController : KSWindow
{
    public Button button_next;
    public bool is_to_setting = false;
    // Start is called before the first frame update
    void Start()
    {
        button_next.onClick.AddListener(OnNextClick);
    }

    void OnNextClick()
    {
        if (this.is_to_setting == true)
        {
            //KSNavigator.Instance.PopCtrl(navigator_bar.configure);
            KSNavigator.Instance.PushCtrl<UITrainTroopController>(new KSKitConfigure(KSNavigatorBarType.nomarl, KSDisplayLayerType.only, KSSortingLayer.Window));
        }
        else
        {
            int[] layers = { KSLayer.model };
            KSNavigator.Instance.PushCtrl<UIThreeDimensionalController>(new KSKitConfigure(KSNavigatorBarType.nomarl, KSDisplayLayerType.only, KSSortingLayer.Model, layers));
        }
    }
}
