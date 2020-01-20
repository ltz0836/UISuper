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

    private KSNavigatorBar _navigator_bar;
    public KSNavigatorBar navigator_bar
    {
        get
        {
            if (_navigator_bar == null)
            {
                _navigator_bar = KSNavigatorBar.Init();
            }
            return _navigator_bar;
        }
    }


    private Stack<GameObject> windows_stack = new Stack<GameObject>();


    public T Push<T>() where T : KSWindow
    {
        GameObject canvas_go = canvas_manager.InstantiateUICanvas<T>();
        GameObject navigator_bar_go = navigator_bar.InstantiateNavigatorBar();
        GameObject window_go = new GameObject();
        windows_stack.Push(window_go);

        window_go.transform.parent = canvas_go.transform;
        navigator_bar_go.transform.parent = canvas_go.transform;

        window_go.ResetRectTransform();
        navigator_bar_go.UpdateNavigatorBarRect(60);
        return null;
    }
}

public class KSCameraManager
{
    public Camera ui_camera;
    public Camera effect_camera;

    public GameObject ui_camera_go;
    public GameObject effect_camera_go;

    public static KSCameraManager Init()
    {
        KSCameraManager camera_manager = new KSCameraManager();
        camera_manager.ui_camera_go = (GameObject)Resources.Load("Camera/UICamera");
        camera_manager.effect_camera_go = (GameObject)Resources.Load("Camera/EffectCamera");

        camera_manager.ui_camera = camera_manager.ui_camera_go.GetComponent<Camera>();
        camera_manager.effect_camera = camera_manager.effect_camera_go.GetComponent<Camera>();
        return camera_manager;
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