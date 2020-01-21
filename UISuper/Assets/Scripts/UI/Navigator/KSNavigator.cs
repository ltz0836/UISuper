using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSNavigator : KSSingleton<KSNavigator>
{
    private KSCameraManager _camera_manager;
    public KSCameraManager camera_manager
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
    public KSCanvasManager canvas_manager
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

    private KSNavigatorBarManager _navigator_bar;
    public KSNavigatorBarManager navigator_bar
    {
        get
        {
            if (_navigator_bar == null)
            {
                _navigator_bar = KSNavigatorBarManager.Init();
            }
            return _navigator_bar;
        }
    }

    private Stack<KSCanvas> canvas_stack = new Stack<KSCanvas>();

    public T Push<T>(KSKitConfigure configure) where T : KSWindow
    {
        KSCanvas ui_canvas = canvas_manager.InstantiateUICanvas<T>(camera_manager.ui_camera);
        ui_canvas.configure = configure;
        ui_canvas.transform.SetSiblingIndex(canvas_stack.Count);
        canvas_stack.Push(ui_canvas);

        T prefab_component = KSWindow.CreatePrefab<T>();

        KSNavigatorBar navigator_bar_component = navigator_bar.InstantiateNavigatorBar();
        navigator_bar_component.key = configure.key;
        prefab_component.navigator_bar = navigator_bar_component;

        prefab_component.transform.SetParent(ui_canvas.transform, false);
        navigator_bar_component.transform.SetParent(ui_canvas.transform, false);
        //prefab_go.ResetRectTransform();
        //navigator_bar_go.UpdateNavigatorBarRect(60);

        return prefab_component;
    }

    public void Pop(string key)
    {
        if (canvas_stack.Count > 0)
        {
            KSCanvas canvas = canvas_stack.Peek();
            if(canvas.configure.key == key)
            {
                canvas = canvas_stack.Pop();
                Object.Destroy(canvas.gameObject);
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
}

public class KSCanvasManager
{
    private GameObject _ui_canvas_go;
    public GameObject ui_canvas_go
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

    public KSCanvas InstantiateUICanvas<T>(Camera worldCamera) where T : KSWindow
    {
        GameObject ui_canvas_gob = UnityEngine.Object.Instantiate(ui_canvas_go);
        ui_canvas_gob.name = typeof(T).Name;
        ui_canvas_gob.GetComponent<Canvas>().worldCamera = worldCamera;
        return ui_canvas_gob.GetComponent<KSCanvas>();
    }
}

public class KSNavigatorBarManager
{
    public GameObject navigator_bar_go;

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