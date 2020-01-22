using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ViewSorcerer : MonoBehaviour
{
    public SortingGroup sorting_group;
    // Start is called before the first frame update
    void Start()
    {
        Canvas canvas = this.transform.parent.parent.GetComponent<Canvas>();
        sorting_group.sortingOrder = canvas.sortingOrder + 1;
        sorting_group.sortingLayerName = KSSortingLayer.Effect;
    }

}
