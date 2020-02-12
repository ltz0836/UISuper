using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInfo : MonoBehaviour
{
    public Image image_level;
    public Text text_name;

    public void UpdateInfo(int level, string name)
    {
        text_name.text = name;
        string path = @"file://" + Application.dataPath + @"/Resources/Images/TrainTroop/UIRomeNum/Num_lv_" + level + ".png";
        StartCoroutine(KSLoadImage.LoadImage(image_level, path));
    }
}
