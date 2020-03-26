using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAreaItem : MonoBehaviour
{
    public Button button_slected;
    /*
     导入一个不规则图片，设置该图片的“Texture Type”为“Sprite（2D and UI）”，并且勾选上“Read/Write Enabled”，然后“Apply”
    */
    public Image image_area;
    public bool isSelected;
    public int index;
    // Start is called before the first frame update
    void Start()
    {
        image_area.alphaHitTestMinimumThreshold = 0.001f;
    }

    public void UpdateState(bool _isSelected)
    {
        image_area.color = _isSelected ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
