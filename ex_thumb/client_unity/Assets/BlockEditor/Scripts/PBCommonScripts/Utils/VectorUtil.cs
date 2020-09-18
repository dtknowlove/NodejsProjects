using UnityEngine;

public class VectorUtil
{
    public static bool AlmostEqual(Vector3 v1, Vector3 v2, float precision)
    {
        bool equal;

        equal = true;

        if (Mathf.Abs(v1.x - v2.x) > precision) equal = false;
        if (Mathf.Abs(v1.y - v2.y) > precision) equal = false;
        if (Mathf.Abs(v1.z - v2.z) > precision) equal = false;

        return equal;
    }
}