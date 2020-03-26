using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[KSPrefabPath("Prefabs/UI/Map/UIMapController")]
public class UIMapController : KSWindow
{
    public List<UIAreaItem> items;
    public Button button_next;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < items.Count; i++)
        {
            UIAreaItem item = items[i];
            item.index = i;
            if(i == 0)
            {
                item.UpdateState(true);
            }
            item.button_slected.onClick.AddListener(()=>OnButtonClick(item));
        }
        button_next.onClick.AddListener(OnNextClick);
    }

    void OnButtonClick(UIAreaItem item)
    {
        foreach(UIAreaItem temp in items)
        {
            temp.UpdateState(temp.index == item.index);
        }
    }
    void OnNextClick()
    {
        KSNavigator.Instance.PushCtrl<UISettingController>(new KSKitConfigure(KSNavigatorBarType.nomarl)); 
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
