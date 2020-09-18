
using System;
using System.Collections.Generic;
using System.Linq;
using PTGame.Core;
//using UnityEditor;
using UnityEngine;

public class StickerData
{
    public string Id { get; set; }
    public string prefab { get; set; }
    public string texture { get; set; }
    public string model { get; set; }
    public string material { get; set; }
	
    public string blockModels { get; set; }
}

public class StickerItem
{
    public string stickerName;
    public Texture2D texture;
    public string prefab;
    public string material;
    public List<string> models;
}

public class StickerGroup : PTSingleton<StickerGroup>
{
    private readonly Dictionary<string, StickerItem> stickerDatas = new Dictionary<string, StickerItem>();
    public List<StickerItem> stickerItems { get; private set; }
//    private static StickerGroup mStickerGroup;
    
    private void AddStickerData(StickerData data)
    {
        StickerItem item;
        if (!stickerDatas.TryGetValue(data.texture, out item))
        {
            item = new StickerItem();
            item.stickerName = data.texture;
            item.prefab = data.prefab;
            item.material = data.material;
            stickerDatas.Add(item.stickerName, item);
            
            string[] strs = data.blockModels.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
            item.models = strs.ToList();
        }
        else
        {
            Debug.LogError("has multiple >>>>>>>"+data.texture);
        }

//        item.prefab.Add(data.prefab);
//        item.material.Add(data.material);
    }
    
    private void LoadTextures()
    {
        if (stickerItems != null)
            return;
        
        stickerItems = stickerDatas.Values.ToList();
        stickerItems.Sort((a, b) => String.CompareOrdinal(a.stickerName, b.stickerName));
        #if UNITY_EDITOR
        foreach (StickerItem item in stickerItems)
        {
            string texPath = string.Format("{0}/{1}.png", BlockPath.Sticker_Texture_Dir, item.stickerName);
            Texture2D td = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
            if (td == null)
            {
                Debug.LogErrorFormat("在 {0} 中找不到这个贴图: {1}.png", BlockPath.Sticker_Texture_Dir, item.stickerName);
            }
            item.texture = td;
        }
        #endif
    }

    public StickerItem GetStickerItemWithPrefabName(string prefabName)
    {
        return stickerItems.FirstOrDefault(s => s.prefab == prefabName);
    }

    private StickerGroup()
    {
        Init();
    }

    private void Init()
    {
//        if (mStickerGroup == null)
        {
//            StickerGroup group = new StickerGroup();
            List<StickerData> datas = PBDataBaseManager.Instance.GetStickerDatas();
            foreach (StickerData stickerData in datas)
            {
                AddStickerData(stickerData);
            }
            LoadTextures();
        }
    }

 

//    public static StickerGroup Load()
//    {
//        StickerGroup group = new StickerGroup();
//        List<StickerData> datas = PBDataBaseManager.Instance.GetStickerDatas();
//        foreach (StickerData stickerData in datas)
//        {
//            group.AddStickerData(stickerData);
//        }
//        
//        group.LoadTextures();
//        return group;
//    }
}