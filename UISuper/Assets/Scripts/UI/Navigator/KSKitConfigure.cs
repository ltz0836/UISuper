using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSKitConfigure
{
    private bool _is_custom_key;
    public bool is_custom_key
    { get { return _is_custom_key; } }

    private string _key;
    public string key
    { get { return _key; } }

    private KSNavigatorBarType _bar_type;
    public KSNavigatorBarType bar_type
    { get { return _bar_type; } }

    private KSDisplayLayerType _display_layer_type;
    public KSDisplayLayerType display_layer_type
    { get { return _display_layer_type; } }

    private bool _is_custom_sorting_layer;
    public bool is_custom_sorting_layer
    { get { return _is_custom_sorting_layer; } }

    private string _sorting_layer;
    public string sorting_layer
    { get { return _sorting_layer; } }

    private int[] _extra_layers;
    public int[] extra_layers
    { get { return _extra_layers; } }

    public KSKitConfigure(KSNavigatorBarType barType,
        KSDisplayLayerType displayLayerType = KSDisplayLayerType.only,
        string sortingLayer = null,
        int[] extraLayers = null,
        string keyValue = null)
    {
        this._bar_type = barType;
        this._display_layer_type = displayLayerType;
        
        this._is_custom_sorting_layer = sortingLayer == null;
        if (sortingLayer != null)
        {
            this._sorting_layer = sortingLayer;
        }
        else
        {
            this._sorting_layer = KSSortingLayer.UI;
        }

        if(extraLayers != null && extraLayers.Length > 0)
        {
            _extra_layers = extraLayers;
        }

        this._is_custom_key = keyValue == null;
        this._key = keyValue;
    }

    public void UpdateKey(string keyValue)
    {
        _key = keyValue;
    }
}

public enum KSNavigatorBarType
{
    nomarl,
    popup,
}

public enum KSDisplayLayerType
{
    only,//只显示当前层
    cover,//覆盖
}

public static class KSLayer
{
    public const int def = 0;
    public const int ui = 5;
    public const int model = 8;
    public const int buildings = 9;
    public const int mainui = 10;
    public const int tips = 31;
    public const int speaker = 31;
    public const int maxui = 19;//最多支持19级UI跳转
}


public static class KSSortingLayer
{
    public const string Default = "Default";
    public const string UI = "UILayer";
    public const string Model = "ModelLayer";
    public const string Effect = "EffectLayer";
    public const string Window = "WindowLayer";
}
