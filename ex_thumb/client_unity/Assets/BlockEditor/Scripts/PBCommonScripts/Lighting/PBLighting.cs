using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class PBLightingData
{
    public Vector3 pos;
    public Vector3 euler;

    public LightType type;
    public Color color;
//    public LightmapBakeType mode;     play mode is not supported
    public float intensity;
    public float indirectMultiplier;
    public LightShadows shadowType;
    public float shadowStrength;
    public LightShadowResolution shadowResolution;
    public float shadowBias;
    public float shadowNormalBias;
    public float shadowNearPlane;
    public LightRenderMode renderMode;
    public int cullingMask;
}

[Serializable]
public class PBLightingDatas
{
    public List<PBLightingData> lightingList = new List<PBLightingData>();
}

public class PBLighting
{
    public const string NAME = "pblight";
    public const string DEFAULT = "default";
    
    public static string Serialize()
    {
        GameObject parentObj = GameObject.Find(NAME);
        if (parentObj == null)
            throw new Exception(">>>> pblight is not found!");

        PBLightingDatas lights = new PBLightingDatas();
        Light[] lightComs = parentObj.GetComponentsInChildren<Light>();
        foreach (Light lightCom in lightComs)
        {
            PBLightingData light = new PBLightingData
            {
                pos = lightCom.transform.position,
                euler = lightCom.transform.eulerAngles,
                
                type =  lightCom.type,
                color = lightCom.color,
                intensity = lightCom.intensity,
                indirectMultiplier = lightCom.bounceIntensity,
                shadowType = lightCom.shadows,
                shadowStrength = lightCom.shadowStrength,
                shadowResolution = lightCom.shadowResolution,
                shadowBias = lightCom.shadowBias,
                shadowNormalBias = lightCom.shadowNormalBias,
                shadowNearPlane = lightCom.shadowNearPlane,
                renderMode = lightCom.renderMode,
                cullingMask = lightCom.cullingMask
            };
            lights.lightingList.Add(light);
        }
        return JsonUtility.ToJson(lights, true);
    }

    public static GameObject Load(string jsonStr)
    {
        GameObject parentObj = GameObject.Find(NAME);
        if (parentObj == null)
            parentObj = new GameObject(NAME);
        
        Transform parentTrans = parentObj.transform;
        parentTrans.position = Vector3.zero;
        parentTrans.rotation = Quaternion.identity;
        parentTrans.localScale = Vector3.one;

        while (parentTrans.childCount > 0)
            GameObject.DestroyImmediate(parentTrans.GetChild(0).gameObject);

        PBLightingDatas lights = JsonUtility.FromJson<PBLightingDatas>(jsonStr);
        foreach (PBLightingData light in lights.lightingList)
        {
            GameObject lightObj = new GameObject(light.type.ToString() + " Light");
            lightObj.transform.parent = parentTrans;
            lightObj.transform.position = light.pos;
            lightObj.transform.eulerAngles = light.euler;
            lightObj.transform.localScale = Vector3.one;

            Light lightCom = lightObj.AddComponent<Light>();
            lightCom.type = light.type;
            lightCom.color = light.color;
            lightCom.intensity = light.intensity;
            lightCom.bounceIntensity = light.indirectMultiplier;
            lightCom.shadows = light.shadowType;
            lightCom.shadowStrength = light.shadowStrength;
            lightCom.shadowResolution = light.shadowResolution;
            lightCom.shadowBias = light.shadowBias;
            lightCom.shadowNormalBias = light.shadowNormalBias;
            lightCom.shadowNearPlane = light.shadowNearPlane;
            lightCom.renderMode = light.renderMode;
            lightCom.cullingMask = light.cullingMask;
        }

        return parentObj;
    }

    public static string FormalPath
    {
        get { return Path.Combine(Application.dataPath, "BlockData/Block_Lights"); }
    }

    public static string BetaPath
    {
        get { return Path.Combine(Application.dataPath, "BlockRes/Custom/Block_Lights"); }
    }

    public static string AppPath
    {
        get { return Path.Combine(Application.persistentDataPath, "config_blockdata/block_lights"); }
    }

    public static GameObject LoadByName_App(string lightName)
    {
        string path = Path.Combine(AppPath, lightName + ".json");
        return Load(File.ReadAllText(path));
    }

    public static void LoadByName_Editor(string lightName)
    {
#if UNITY_EDITOR
        string path = Path.Combine(FormalPath, lightName + ".json");
        if (!File.Exists(path))
            path = Path.Combine(BetaPath, lightName + ".json");
        if (!File.Exists(path))
            throw new Exception("找不到灯光配置：" + lightName);

        Load(File.ReadAllText(path));
#endif
    }
}
