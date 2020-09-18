using UnityEditor;
using UnityEngine;

public class HierarchyMenu
{
    [MenuItem("GameObject/PEBlocks/Calculate Size", false, 0)]
    private static void CalculateSize()
    {
        float factor = 39.5741427f;
        
        Renderer[] rList = Selection.activeGameObject.GetComponentsInChildren<Renderer>();
        Bounds b = new Bounds(rList[0].bounds.center, rList[0].bounds.size);
        foreach (Renderer r in rList)
        {
            //排除虚拟体
            if (r.name.Equals("refpoint")) continue;
            b.Encapsulate(r.bounds);
        }

        float x = b.size.x * factor;
        float y = b.size.y * factor;
        float z = b.size.z * factor;
        
        string text = string.Format("{0}\n的size为：\nx: {1:0} \ny: {2:0} \nz: {3:0}", Selection.activeGameObject.name, x, y, z);
        EditorUtility.DisplayDialog(null, text, "OK");
    }
}
