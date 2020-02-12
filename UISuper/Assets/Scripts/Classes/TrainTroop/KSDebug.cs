using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class KSDebug
{
    public static bool enableLog = false;
    public static void LogError(object message)
    {
#if UNITY_EDITOR
        Debug.LogError("========= ASX Error: " + message + " =========");
#endif
    }
    public static void Log(object message)
    {
#if UNITY_EDITOR
        Debug.Log("========= ASX Log: " + message + " =========");
#endif
    }
}
