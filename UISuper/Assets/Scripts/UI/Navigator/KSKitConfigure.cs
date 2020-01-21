using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSKitConfigure
{
    private string _key;
    public string key
    { get { return _key; } }
    private bool _is_custom_key;
    public bool is_custom_key
    { get { return _is_custom_key; } }
    private KSNavigatorBarType _bar_type;
    public KSNavigatorBarType bar_type
    { get { return _bar_type; } }
    private KSCameraType _camera_type;
    public KSCameraType camera_type
    { get { return _camera_type; } }

    public void UpdateKey(string keyValue)
    {
        _key = keyValue;
    }

    public KSKitConfigure(KSCameraType cameraType, KSNavigatorBarType barType, bool isCustomKey = false, string keyValue = null)
    {
        this._bar_type = barType;
        this._camera_type = cameraType;
        this._is_custom_key = isCustomKey;
        this._key = keyValue;
    }
}

public enum KSNavigatorBarType
{
    nomarl,
    popup,
}

public enum KSCameraType
{
    ui,
    effect,
}

public static class KSSortingLayer
{
    public const string Default = "Default";
    public const string UI = "UI";
    public const string Effect = "Effect";
    public const string Window = "Window";

    public static string SortingLayer(KSCameraType type)
    {
        switch (type)
        {
            case KSCameraType.ui:
                return UI;
            case KSCameraType.effect:
                return Effect;
            default:
                return Default;
        }
    }
    public static int SortingOrder(KSCameraType type)
    {
        switch (type)
        {
            case KSCameraType.ui:
                return 0;
            case KSCameraType.effect:
                return 1;
            default:
                return 0;
        }
    }
}