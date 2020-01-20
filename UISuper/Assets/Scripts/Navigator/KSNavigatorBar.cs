using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KSNavigatorBar : MonoBehaviour
{
    public Button button_back;

    private void Start()
    {
        button_back.onClick.AddListener(OnBackClick);
    }

    void OnBackClick()
    {
        KSNavigator.Instance.Pop();
    }
}
