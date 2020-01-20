using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class KSExtension
{
    public static void ResetRectTransform(this GameObject go)
    {
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.localScale = new Vector3(1, 1, 1);
        rect.offsetMax = new Vector2(0, 0);
        rect.offsetMin = new Vector2(0, 0);
        rect.anchoredPosition3D = new Vector3(0, 0, 0);
    }

    public static void UpdateNavigatorBarRect(this GameObject go, float height)
    {
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.localScale = new Vector3(1, 1, 1);
        rect.sizeDelta = new Vector2(0, height);
        rect.anchoredPosition3D = new Vector3(0, -(height / 2), 0);
    }
}