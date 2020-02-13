using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISoldierItem : MonoBehaviour
{
    public Image image;

    public void SetArmyConfig(int index, ArmyConfig config)
    {
        string path = @"file://" + Application.dataPath + @"/Resources/Images/TrainTroop/Army/" + config.image_name + ".png";
        StartCoroutine(KSLoadImage.LoadImage(image, path));
    }

    public void OnSelectedChanged(int selectIndex)
    {

    }

    public void OnScrollStatusChanged(UITurnTable.TableStatus oldStatus, UITurnTable.TableStatus newStatus)
    {

    }
}
