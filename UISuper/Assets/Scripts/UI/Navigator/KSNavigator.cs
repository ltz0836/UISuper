using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KSNavigator : KSSingleton<KSNavigator>
{
    private KSCameraManager _camera_manager;
    private KSCameraManager camera_manager
    {
        get
        {
            if (_camera_manager == null)
            {
                _camera_manager = KSCameraManager.Init();
            }
            return _camera_manager;
        }
    }

    private KSCanvasManager _canvas_manager;
    private KSCanvasManager canvas_manager
    {
        get
        {
            if (_canvas_manager == null)
            {
                _canvas_manager = KSCanvasManager.Init();
            }
            return _canvas_manager;
        }
    }

    private KSNavigatorBarManager _navigator_bar_manager;
    private KSNavigatorBarManager navigator_bar_manager
    {
        get
        {
            if (_navigator_bar_manager == null)
            {
                _navigator_bar_manager = KSNavigatorBarManager.Init();
            }
            return _navigator_bar_manager;
        }
    }

    private Dictionary<KSCameraType, Stack<KSCanvas>> canvas_dict = new Dictionary<KSCameraType, Stack<KSCanvas>>();

    public T PushCtrl<T>(KSKitConfigure configure) where T : KSWindow
    {
        //1、更新标识Key
        if (configure.is_custom_key == false)
        {
            configure.UpdateKey(typeof(T).Name);
        }

        //2、获取Camera
        Camera world_camera = camera_manager.GetCamera(configure.camera_type);

        //3、获取Canvas
        //int index = canvas_dict.ContainsKey(configure.camera_type) ? canvas_dict[configure.camera_type].Count : 0;
        KSCanvas ui_canvas = canvas_manager.InstantiateUICanvas<T>(world_camera, configure, SortingOrder());
        //入栈
        PushCanvas(ui_canvas);

        //4、页面组件
        T prefab_component = KSWindow.CreatePrefab<T>();

        //5、导航组件
        KSNavigatorBar navigator_bar_component = null;
        switch (configure.bar_type)
        {
            case KSNavigatorBarType.nomarl:
                navigator_bar_component = navigator_bar_manager.InstantiateNavigatorBar();
                navigator_bar_component.UpdateConfigure(configure);
                prefab_component.navigator_bar = navigator_bar_component;
                break;
            case KSNavigatorBarType.popup:
                break;
        }
        //6、设置父物体
        prefab_component.transform.SetParent(ui_canvas.transform, false);
        if (navigator_bar_component != null)
        {
            navigator_bar_component.transform.SetParent(ui_canvas.transform, false);
        }
        return prefab_component;
    }

    private int SortingOrder()
    {
        int index = 0;
        foreach (Stack<KSCanvas> stack in canvas_dict.Values)
        {
            index += stack.Count;
        }
        return index;
    }

    private void PushCanvas(KSCanvas canvas)
    {
        if (canvas_dict.ContainsKey(canvas.configure.camera_type) == false)
        {
            canvas_dict[canvas.configure.camera_type] = new Stack<KSCanvas>();
        }
        canvas_dict[canvas.configure.camera_type].Push(canvas);
    }

    public void DismissCtrl(KSCameraType type)
    {
        if (canvas_dict.ContainsKey(type))
        {
            foreach (KSCanvas canvas in canvas_dict[type])
            {
                GameObject.Destroy(canvas.gameObject);
            }
            camera_manager.DestroyCamera(type);
        }
    }

    public void ToRootCtrl(KSCameraType type)
    {
        if (canvas_dict.ContainsKey(type))
        {
            if (canvas_dict[type].Count <= 1)
            {
                return;
            }
            while (canvas_dict[type].Count > 1)
            {
                KSCanvas canvas = canvas_dict[type].Pop();
                GameObject.Destroy(canvas.gameObject);
            }
        }
    }

    public void ToCtrl(string key, KSCameraType type)
    {
        if (canvas_dict.ContainsKey(type))
        {
            foreach (KSCanvas canvas in canvas_dict[type])
            {
                if (canvas.configure.key == key)
                {
                    bool isPop = true;
                    while (isPop)
                    {
                        KSCanvas temp = canvas_dict[type].Pop();
                        GameObject.Destroy(temp.gameObject);
                        if (temp.configure.key == key)
                        {
                            isPop = false;
                            if (canvas_dict[type].Count == 0)
                            {
                                camera_manager.DestroyCamera(type);
                            }
                            return;
                        }
                    }
                }
            }
        }
    }

    public void ToCtrl<T>(KSCameraType type) where T : KSWindow
    {
        string key = typeof(T).Name;
        ToCtrl(key, type);
    }

    public void PopCtrl(KSNavigatorBarConfigure configure)
    {
        KSCameraType type_key = configure.camera_type;
        if (canvas_dict.ContainsKey(type_key) && canvas_dict[type_key].Count > 0)
        {
            KSCanvas canvas = canvas_dict[type_key].Peek();
            if (canvas.configure.key == configure.key && canvas.configure.camera_type == type_key)
            {
                canvas = canvas_dict[type_key].Pop();
                Object.Destroy(canvas.gameObject);
                if (canvas_dict[type_key].Count == 0)
                {
                    camera_manager.DestroyCamera(type_key);
                }
            }
        }
    }
}

public class KSCameraManager
{
    private Camera _ui_camera;
    private Camera _effect_camera;

    private GameObject _ui_camera_go;
    private GameObject _effect_camera_go;

    public Camera ui_camera
    {
        get
        {
            if (_ui_camera == null)
            {
                _ui_camera = ui_camera_go.GetComponent<Camera>();
            }
            return _ui_camera;
        }
    }
    public Camera effect_camera
    {
        get
        {
            if (_effect_camera == null)
            {
                _effect_camera = effect_camera_go.GetComponent<Camera>();
            }
            return _effect_camera;
        }
    }

    public GameObject ui_camera_go
    {
        get
        {
            if (_ui_camera_go == null)
            {
                _ui_camera_go = (GameObject)Resources.Load(KSPrefabAssets.UICamera);
                _ui_camera_go = UnityEngine.Object.Instantiate(_ui_camera_go);
                _ui_camera_go.name = "UICamera";
            }
            return _ui_camera_go;
        }
    }
    public GameObject effect_camera_go
    {
        get
        {
            if (_effect_camera_go == null)
            {
                _effect_camera_go = (GameObject)Resources.Load(KSPrefabAssets.EffectCamera);
                _effect_camera_go = UnityEngine.Object.Instantiate(_effect_camera_go);
                _effect_camera_go.name = "EffectCamera";
            }
            return _effect_camera_go;
        }
    }

    public static KSCameraManager Init()
    {
        KSCameraManager camera_manager = new KSCameraManager();
        return camera_manager;
    }

    public void SetUICanvas(GameObject canvas)
    {
        canvas.GetComponent<Canvas>().worldCamera = ui_camera;
    }

    public Camera GetCamera(KSCameraType type)
    {
        switch (type)
        {
            case KSCameraType.ui:
                return ui_camera;
            case KSCameraType.effect:
                return effect_camera;
            default:
                return ui_camera;
        }
    }

    public void DestroyCamera(KSCameraType type)
    {
        switch (type)
        {
            case KSCameraType.ui:
                if (_ui_camera_go != null)
                {
                    UnityEngine.Object.Destroy(_ui_camera_go);
                    if (_ui_camera != null)
                    {
                        _ui_camera = null;
                    }
                }
                break;
            case KSCameraType.effect:
                if (_effect_camera_go != null)
                {
                    UnityEngine.Object.Destroy(_effect_camera_go);
                    if (_effect_camera != null)
                    {
                        _effect_camera = null;
                    }
                }
                break;
        }
    }


    //当前是否有任意窗口显示
    private bool is_any_window_showing = false;
    //canvas 的遮挡层次
    private int max_sort_layer = 0;

    private bool force_hide_mainui = false;

    private const int sort_layer_inc = 30;
    //摄像头的渲染层次，11~31，决定该层是否被摄像头渲染，ui是从11开始
    private int _max_layer = mainui_layer + 1;
    public int max_layer
    { get { return _max_layer; } }

    private const int mainui_layer = 10;
    private const int guide_layer = 30;
    private const int tips_layer = 31;
    private const int speaker_layer = 31;

    public void UpdateCameraCullingMask(Stack<KSCanvas> stack, KSCanvas currentCanvas, Camera uiCamera)
    {
        int startLeayer = currentCanvas.configure.layer_index;
        bool hideMainUI = currentCanvas.IsCoverMainUI();
        hideMainUI |= force_hide_mainui;

        int mask = 0;
        int uiMask = 0;

        for (int i = startLeayer; i < _max_layer; i++)
        {
            if (i == mainui_layer && hideMainUI)
            {
                continue;
            }
            if (i < mainui_layer)
            {
                mask |= 1 << i;
            }
            else
            {
                uiMask |= 1 << i;
            }
        }
        //tips 和 guide一直都显示
        uiMask |= 1 << guide_layer;
        uiMask |= 1 << tips_layer;
        uiMask |= 1 << speaker_layer;

        //存在单独的ui camera
        Camera.main.cullingMask = mask;
        uiCamera.cullingMask = uiMask;

        GraphicRaycaster caster = currentCanvas.gameObject.AddComponent<GraphicRaycaster>();
        caster.enabled = !hideMainUI;
    }
}

public class KSCanvasManager
{
    private GameObject _ui_canvas_go;
    private GameObject ui_canvas_go
    {
        get
        {
            return _ui_canvas_go;
        }
    }

    public static KSCanvasManager Init()
    {
        KSCanvasManager canvas_manager = new KSCanvasManager();
        canvas_manager._ui_canvas_go = (GameObject)Resources.Load(KSPrefabAssets.UICanvas);
        return canvas_manager;
    }

    public KSCanvas InstantiateUICanvas<T>(Camera worldCamera, KSKitConfigure configure, int index) where T : KSWindow
    {
        GameObject ui_canvas_gob = UnityEngine.Object.Instantiate(ui_canvas_go);
        ui_canvas_gob.name = typeof(T).Name + index;

        Canvas canvas = ui_canvas_gob.GetComponent<Canvas>();
        canvas.worldCamera = worldCamera;
        canvas.sortingOrder = index;
        canvas.sortingLayerName = KSSortingLayer.SortingLayer(configure.camera_type);
        canvas.transform.SetSiblingIndex(index);
        KSCanvas canvas_component = ui_canvas_gob.GetComponent<KSCanvas>();
        canvas_component.configure = configure;
        return canvas_component;
    }
}

public class KSNavigatorBarManager
{
    private GameObject navigator_bar_go;

    public static KSNavigatorBarManager Init()
    {
        KSNavigatorBarManager navigator_bar = new KSNavigatorBarManager();
        navigator_bar.navigator_bar_go = (GameObject)Resources.Load(KSPrefabAssets.Navigator);
        return navigator_bar;
    }

    public KSNavigatorBar InstantiateNavigatorBar()
    {
        GameObject navigator_bar_gob = UnityEngine.Object.Instantiate(navigator_bar_go);
        navigator_bar_gob.name = "KSNavigatorBar";
        return navigator_bar_gob.GetComponent<KSNavigatorBar>();
    }
}

public static class KSPrefabAssets
{
    public const string UICamera = "Prefabs/Camera/UICamera";
    public const string EffectCamera = "Prefabs/Camera/EffectCamera";

    public const string Navigator = "Prefabs/Navigator/NavigatorBar";

    public const string UICanvas = "Prefabs/Canvas/UICanvas";

}
