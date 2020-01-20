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

    private Stack<GameObject> canvas_stack = new Stack<GameObject>();

    public T Push<T>() where T : KSWindow
    {
        GameObject canvas_go = canvas_manager.InstantiateUICanvas<T>();
        camera_manager.SetUICanvas(canvas_go);
        canvas_stack.Push(canvas_go);
        canvas_go.name = canvas_go.name + canvas_stack.Count;
        canvas_go.transform.SetSiblingIndex(canvas_stack.Count);

        string prefab_psth = KSExtension.GetPrefabPath<T>();
        GameObject prefab_go = (GameObject)Resources.Load(prefab_psth);
        prefab_go = Instantiate(prefab_go);
        prefab_go.name = typeof(T).Name;

        GameObject navigator_bar_go = navigator_bar.InstantiateNavigatorBar();

        prefab_go.transform.SetParent(canvas_go.transform);
        navigator_bar_go.transform.SetParent(canvas_go.transform);

        prefab_go.ResetRectTransform();
        navigator_bar_go.UpdateNavigatorBarRect(60);

        return null;
    }

    public void Pop()
    {
        if(canvas_stack.Count > 0)
        {
            GameObject canvas_go = canvas_stack.Pop();
            GameObject.Destroy(canvas_go);
        }
    }
}

public class KSCameraManager
{
    public Camera _ui_camera;
    public Camera _effect_camera;

    public GameObject _ui_camera_go;
    public GameObject _effect_camera_go;

    public Camera ui_camera {
        get {
            if(_ui_camera == null)
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
            if(_effect_camera == null)
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
            if(_ui_camera_go == null)
            {
                _ui_camera_go = (GameObject)Resources.Load("Camera/UICamera");
                _ui_camera_go = UnityEngine.Object.Instantiate(_ui_camera_go);
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
                _effect_camera_go = (GameObject)Resources.Load("Camera/UICamera");
                _effect_camera_go = UnityEngine.Object.Instantiate(_effect_camera_go);
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
        ui_camera_go.name = "UICamera";
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
        canvas_manager._ui_canvas_go = (GameObject)Resources.Load("Canvas/UICanvas");
        return canvas_manager;
    }

    public GameObject InstantiateUICanvas<T>() where T : KSWindow
    {
        GameObject ui_canvas_gob = UnityEngine.Object.Instantiate(ui_canvas_go);
        ui_canvas_gob.name = typeof(T).Name;
        return ui_canvas_gob;
    }
}

public class KSNavigatorBarManager
{
    public GameObject navigator_bar_go;

    public static KSNavigatorBarManager Init()
    {
        KSNavigatorBarManager navigator_bar = new KSNavigatorBarManager();
        navigator_bar.navigator_bar_go = (GameObject)Resources.Load("Navigator/NavigatorBar");
        return navigator_bar;
    }

    public GameObject InstantiateNavigatorBar()
    {
        GameObject navigator_bar_gob = UnityEngine.Object.Instantiate(navigator_bar_go);
        navigator_bar_gob.name = typeof(KSNavigatorBar).Name;
        return navigator_bar_gob;
    }
}
