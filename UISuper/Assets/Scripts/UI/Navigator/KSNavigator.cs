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
            { _camera_manager = KSCameraManager.Init(); }
            return _camera_manager;
        }
    }

    private KSCanvasManager _canvas_manager;
    private KSCanvasManager canvas_manager
    {
        get
        {
            if (_canvas_manager == null)
            { _canvas_manager = KSCanvasManager.Init(); }
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

    private Stack<KSCanvas> canvas_stack = new Stack<KSCanvas>();

    public T PushCtrl<T>(KSKitConfigure configure) where T : KSWindow
    {
        if(canvas_stack.Count >= KSLayer.maxui)
        {
            return null;
        }

        //1、更新标识Key
        if (configure.is_custom_key == false)
        {
            configure.UpdateKey(typeof(T).Name);
        }

        //2、获取Canvas
        KSCanvas ui_canvas = canvas_manager.InstantiateUICanvas<T>(camera_manager.ui_camera, configure, canvas_stack.Count);

        //3、页面组件
        T prefab_component = KSWindow.CreateWindow<T>();

        //4、导航组件
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

        //5、设置父物体
        prefab_component.transform.SetParent(ui_canvas.transform, false);
        if (navigator_bar_component != null)
        {
            navigator_bar_component.transform.SetParent(ui_canvas.transform, false);
        }

        //6、入栈
        canvas_stack.Push(ui_canvas);

        //7、更新CameraCullingMask
        camera_manager.UpdateCameraCullingMask(canvas_stack);

        return prefab_component;
    }
    public void DismissCtrl()
    {
        while (canvas_stack.Count > 0)
        {
            KSCanvas canvas = canvas_stack.Pop();
            GameObject.Destroy(canvas.gameObject);
        }
        camera_manager.DestroyCamera();
    }
    /*
    public void ToRootCtrl()
    {
        while (canvas_stack.Count > 1)
        {
            KSCanvas canvas = canvas_stack.Pop();
            GameObject.Destroy(canvas.gameObject);
        }
    }

    public void ToCtrl(string key)
    {
        foreach (KSCanvas canvas in canvas_stack)
        {
            if (canvas.configure.key == key)
            {
                bool isPop = true;
                while (isPop)
                {
                    KSCanvas temp = canvas_stack.Pop();
                    GameObject.Destroy(temp.gameObject);
                    if (temp.configure.key == key)
                    {
                        isPop = false;
                        if (canvas_stack.Count == 0)
                        {
                            camera_manager.DestroyCamera();
                        }
                        return;
                    }
                }
            }
        }
    }

    public void ToCtrl<T>() where T : KSWindow
    {
        string key = typeof(T).Name;
        ToCtrl(key);
    }
    */
    public void PopCtrl(KSNavigatorBarConfigure configure)
    {
        if (canvas_stack.Count > 0)
        {
            KSCanvas canvas = canvas_stack.Peek();
            if (canvas.configure.key == configure.key)
            {
                canvas = canvas_stack.Pop();
                Object.Destroy(canvas.gameObject);
                if (canvas_stack.Count == 0)
                {
                    camera_manager.DestroyCamera();
                }
                else
                {
                    // 新CameraCullingMask
                    camera_manager.UpdateCameraCullingMask(canvas_stack);
                }
            }
        }
    }
}

public class KSCameraManager
{
    private Camera _ui_camera;
    private GameObject _ui_camera_go;

    public Camera ui_camera
    {
        get
        {
            if (_ui_camera == null)
            { _ui_camera = ui_camera_go.GetComponent<Camera>(); }
            return _ui_camera;
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

    public static KSCameraManager Init()
    {
        KSCameraManager camera_manager = new KSCameraManager();
        return camera_manager;
    }

    public void DestroyCamera()
    {
        if (_ui_camera_go != null)
        {
            UnityEngine.Object.Destroy(_ui_camera_go);
            if (_ui_camera != null)
            {
                _ui_camera = null;
            }
        }
    }

    public void UpdateCameraCullingMask(Stack<KSCanvas> stack)
    {
        if (stack.Count == 0)
        { return; }

        int flag_layer = stack.Count + KSLayer.mainui;

        KSCanvas lastCanvas = stack.Peek();
        lastCanvas.gameObject.layer = flag_layer;

        int uiMask = 0;
        switch (lastCanvas.configure.display_layer_type)
        {
            case KSDisplayLayerType.only:
                uiMask |= 1 << flag_layer;
                break;
            case KSDisplayLayerType.cover:
                if (stack.Count > 0)
                {
                    uiMask |= 1 << (flag_layer - 1);
                }
                uiMask |= 1 << flag_layer;
                break;
            default:
                break;
        }
        if(lastCanvas.configure.extra_layers != null && lastCanvas.configure.extra_layers.Length > 0)
        {
            foreach(int extraLayer in lastCanvas.configure.extra_layers)
            {
                uiMask |= 1 << extraLayer;
            }
        }
        switch (lastCanvas.configure.sorting_layer)
        {
            case KSSortingLayer.UI:
                break;
            case KSSortingLayer.Model:
                uiMask |= 1 << KSLayer.def;
                break;
            case KSSortingLayer.Effect:
                uiMask |= 1 << KSLayer.buildings;
                break;
            case KSSortingLayer.Window:
                break;
            default:
                break;
        }

        uiMask |= 1 << KSLayer.ui;
        uiMask |= 1 << KSLayer.mainui;
        uiMask |= 1 << KSLayer.tips;
        ui_camera.cullingMask = uiMask;
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
        canvas.sortingLayerName = configure.sorting_layer;
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
    public const string Navigator = "Prefabs/Navigator/NavigatorBar";
    public const string UICanvas = "Prefabs/Canvas/UICanvas";
}
