using System.Collections.Generic;
using System.Linq;
//using UnityEditor;
using UnityEngine;

public class PBTextureData
{
    public string Id { get; set; }
    public string prefab { get; set; }
    public string texture { get; set; }
    public string model { get; set; }
    public string material { get; set; }
}

public class PBTextureItem
{
    public string texName;
    public Texture2D texture;
    public List<string> prefab;
    public List<string> material;
}

public class PBTextureDataGroup
{
    public string modelName;
    private readonly Dictionary<string, PBTextureItem> texDatas = new Dictionary<string, PBTextureItem>();
    public List<PBTextureItem> textures { get; private set; }
    
    public void AddTextureData(PBTextureData data)
    {
        PBTextureItem item;
        if (!texDatas.TryGetValue(data.texture, out item))
        {
            item = new PBTextureItem();
            item.texName = data.texture;
            item.prefab = new List<string>();
            item.material = new List<string>();
            texDatas.Add(data.texture, item);
        }
        item.prefab.Add(data.prefab);
        item.material.Add(data.material);
    }

    public void LoadTextures()
    {
        if (textures == null)
        {
            textures = texDatas.Values.ToList();
            foreach (string texName in texDatas.Keys)
            {
                #if UNITY_EDITOR
                string texPath = string.Format("{0}/{1}.png", BlockPath.Texture_Texture_Dir, texName);
                Texture2D td = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
                if (td == null)
                {
                    Debug.LogError("找不到这个贴图:" + texName + ".png");
                }
                texDatas[texName].texture = td;
                #endif
            }
        }
    }

    public static List<PBTextureDataGroup> Load()
    {
        Dictionary<string, PBTextureDataGroup> groups = new Dictionary<string, PBTextureDataGroup>();
        List<PBTextureData> datas = PBDataBaseManager.Instance.GetTextureDatas();
        foreach (PBTextureData data in datas)
        {
            PBTextureDataGroup group;
            if (!groups.TryGetValue(data.model, out group))
            {
                group = new PBTextureDataGroup();
                group.modelName = data.model;
                groups.Add(data.model, group);
            }
            group.AddTextureData(data);
        }
        
        List<PBTextureDataGroup> groupList = groups.Values.ToList();
        foreach (PBTextureDataGroup g in groupList)
        {
            g.LoadTextures();
        }

        return groupList;
    }
}