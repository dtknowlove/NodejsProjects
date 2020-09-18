using System;
using System.Collections.Generic;
using System.IO;
using LitJson;
using PTGame.Editor.PluginManager;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AppDataExporter
{
//    [Serializable]
//    public class DBBlockData
//    {
//        public string prefab;
//        public string category;
//    }
//
//    public static void OnFinishExportBlockData(string pluginType, string pluginName)
//    {
//        if (string.Equals(pluginType, "blockeditor") && string.Equals(pluginName, "blockdata"))
//        {
//            Export();
//            Selection.activeObject = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>("Assets/BlockRes/AppBlockDB");
//            PTPluginGenerator.ExportPTPlugin();
//        }
//    }
//
//    private static void Export()
//    {
//        PBDataBaseManager.Instance.Init();
//        List<BlockData> blockDatas = PBDataBaseManager.Instance.GetBlockDatas();
//        List<DBBlockData> datas = new List<DBBlockData>();
//        
//        foreach (BlockData data in blockDatas)
//        {
//            datas.Add(new DBBlockData
//            {
//                prefab = data.prefab,
//                category = data.category
//            });
//        }
//        List<StickerData> stickerDatas = PBDataBaseManager.Instance.GetStickerDatas();
//        foreach (StickerData data in stickerDatas)
//        {
//            datas.Add(new DBBlockData
//            {
//                prefab = data.prefab,
//                category = "sticker"
//            });
//        }
//        
//        JsonWriter jw = new JsonWriter();
//        jw.PrettyPrint = true;
//        JsonMapper.ToJson(datas, jw);
//        File.WriteAllText(Path.Combine(Application.dataPath, "BlockRes/AppBlockDB/block_db.json"), jw.ToString());
//        AssetDatabase.Refresh();
//    }
}