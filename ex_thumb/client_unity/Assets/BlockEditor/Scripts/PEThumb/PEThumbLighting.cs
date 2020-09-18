#if UNITY_EDITOR

using System;
using System.IO;
using UnityEngine;

public class PEThumbLighting
{
    public static void SaveForColor(string color)
    {
        PEThumbLight thumbLight = GameObject.FindObjectOfType<PEThumbLight>();
        if (thumbLight == null)
            throw new Exception(">>>> pblight is not found!");
        
        thumbLight.lightName = color;

        string prefabPath = "Assets/BlockEditor/Resources/ThumbLight/" + thumbLight.prefabName + ".prefab";
        UnityEditor.PrefabUtility.CreatePrefab(prefabPath, thumbLight.gameObject, UnityEditor.ReplacePrefabOptions.ReplaceNameBased);
    }

    public static GameObject LoadForColor(string color)
    {
        PEThumbLight thumbLight = GameObject.FindObjectOfType<PEThumbLight>();
        if (thumbLight != null)
            GameObject.DestroyImmediate(thumbLight.gameObject);

        string prefabName = string.IsNullOrEmpty(color) ? "pblight" : "pblight_" + color;
        if (!File.Exists(Path.Combine(Application.dataPath, "BlockEditor/Resources/ThumbLight/" + prefabName + ".prefab")))
            prefabName = "pblight";

        GameObject lightPrefab = Resources.Load<GameObject>("ThumbLight/" + prefabName);
        GameObject lightObj = GameObject.Instantiate(lightPrefab);
        lightObj.name = prefabName;

        return lightObj;
    }
}

#endif