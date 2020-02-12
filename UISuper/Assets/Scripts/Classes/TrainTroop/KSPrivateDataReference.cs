using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSPrivateDataReference : MonoBehaviour
{
    private object data;

    public void setData<T>(T privateData)
    {
        data = privateData;
    }
    public T getData<T>()
    {
        return (T)data;
    }
}
