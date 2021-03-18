using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorComparaison
{
    public static bool VectorComp(Vector3 v1, Vector3 v2)
    {
        if (Mathf.Approximately(v1.x, v2.x) &&
        Mathf.Approximately(v1.y, v2.y) &&
        Mathf.Approximately(v1.z, v2.z))
            return true;
        else
            return false;
    }
}
