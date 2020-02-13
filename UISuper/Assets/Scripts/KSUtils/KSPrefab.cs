using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSPrefabPath : System.Attribute
{
    public string[] paths;
    public KSPrefabPath(params string[] paths)
    {
        this.paths = paths;
    }
}

