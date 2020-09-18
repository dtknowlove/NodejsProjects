using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace PTGame.ResKit
{
    [Serializable]
    public class ResGroupConfig
    {
        public string GroupName;
        public bool DefaultLoad;
        public bool SimulateLoad = true;
    }

    public class ResKitConfig : ScriptableObject
    {
#if UNITY_EDITOR
        [MenuItem("Assets/Create/reskitconfig")]
        static void Create()
        {
            ResKitConfig config = CreateInstance<ResKitConfig>();
            AssetDatabase.CreateAsset(config, "Assets/PTUGame/customconfig/Resources/reskitconfig.asset");
            AssetDatabase.SaveAssets();
        }
#endif

        [SerializeField] public List<ResGroupConfig> ResGroups = new List<ResGroupConfig>();

        public static List<ResGroupConfig> GetResGroups()
        {
            return GetConfig().ResGroups;
        }

        public static ResKitConfig GetConfig()
        {
            ResKitConfig config = Resources.Load<ResKitConfig>("reskitconfig");
            if (config == null)
            {
                Debug.Log(">>>[ResKit]: There is no reskitconfig.asset under \"/PTUGame/customconfig/Resources/\", use default directory");
                return null;
            }
            return config;
        }
    }
}