using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSSingleton<T> : MonoBehaviour where T : KSSingleton<T>
{
    protected static T _instance = null;

    public static T Instance
    {
        get
        {
            //如果_instance为空
            if (_instance == null)
            {
                string name = "KSSingleton";
                //寻找不销毁的组件（可手动创建，也可以通过下面的脚本实现）
                GameObject go = GameObject.Find(name);
                //如果KSSingleton组件为空
                if (go == null)
                {
                    //创建KSSingleton组件
                    go = new GameObject(name);
                    //设置为不可销毁
                    DontDestroyOnLoad(go);
                }
                //在KSSingleton组件上得到T组件
                _instance = go.GetComponent<T>();
                //如果是空，则说明KSSingleton组件没有添加T组件
                if (_instance == null)
                {
                    //给KSSingleton组件添加T组件，并赋值给_instance
                    _instance = go.AddComponent<T>();
                }
            }
            return _instance;
        }
    }
}
