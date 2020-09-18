using System;
using System.Collections.Generic;
using System.Linq;
using PTGame.Core;
using UnityEngine;

[Serializable]
public class RenderFile
{
    public int id;
    public RenderInfo info;
}

[Serializable]
public class RenderInfo
{
    /// <summary>
    /// 单位:240
    /// </summary>
    public int scale;
    public RenderItem[] items;

    public RenderInfo()
    {
        scale = 2;
    }

    public HashSet<string> GetUrls()
    {
        var result = new HashSet<string>();
        foreach (var renderItem in items)
        {
            renderItem.GetUrls().ForEach(t =>
            {
                if (!string.IsNullOrEmpty(t))
                    result.Add(t);
            });
        }
        return result;
    }

    public List<ThumbExportElement> GetExportItems()
    {
        var result = new List<ThumbExportElement>();
        if (items != null && items.Length > 0)
        {
            result.AddRange(items.Select(renderItem => renderItem.ToExport()));
        }
        return result;
    }


    public RenderItem GetRenderItemByModelUrl(string url)
    {
        return items.FirstOrDefault(t => t.EqualsUrl(url));
    }
    
    public RenderItem GetRenderItemByTexUrl(string url)
    {
        return items.FirstOrDefault(t => t.EqualsTexUrl(url));
    }
}

[Serializable]
public class RenderItem
{
    public Vector3 angle;
    public string name;
    public string model;
    public string material;
    public List<MaterialInfo> matinfos;
    public string bin_url;
    public string gltf_url;
    public SkinInfo[] skins;
    public List<PPPrefabTexInfo> texs;

    public RenderItem()
    {
        angle = new Vector3(0, -35, 0);
        matinfos=new List<MaterialInfo>();
        texs = new List<PPPrefabTexInfo>();
    }

    public bool EqualsUrl(string url)
    {
        if (ParentEqualUrl(url))
            return true;
        return texs.Any(t => t.EqualsModelUrl(url));
    }
    
    public bool EqualsTexUrl(string url)
    {
        if (GetSkinInfoByUrl(url) != null)
        {
            return true;
        }
        return GetTexInfoByUrl(url) != null;
    }

    public SkinInfo GetSkinInfoByUrl(string url)
    {
        return skins.FirstOrDefault(t => t.EqualUrl(url));
    }
    
    public PPPrefabTexInfo GetTexInfoByUrl(string url)
    {
        return texs.FirstOrDefault(t => t.skin_url.Equals(url));
    }

    public bool ParentEqualUrl(string url)
    {
        return bin_url.Equals(url) || gltf_url.Equals(url);
    }

    public PPPrefabTexInfo GetTexModelByUrl(string url)
    {
        return texs.FirstOrDefault(t => t.EqualsModelUrl(url));
    }

    public List<string> GetUrls()
    {
        var result = new List<string> {bin_url, gltf_url};
        result.AddRange(skins.Select(t => t.skin_url));
        result.AddRange(texs.Select(t => t.bin_url));
        result.AddRange(texs.Select(t => t.gltf_url));
        result.AddRange(texs.Select(t => t.skin_url));
        return result;
    }

    public ThumbExportElement ToExport()
    {
        //构造prefabinfo
        var prefabInfo = new PPPrefabInfo()
        {
            Name = name,
            Model = model,
            Material = material,
            MaterialInfos = matinfos,
            Texs = texs
        };
        if (prefabInfo.IsSticker)
            prefabInfo.Texture = skins[0].skin_name;
        
        //构造PPBlockInfo
        var nodeInfo = new PPBlockInfo()
        {
            Prefab = name,
        };
        var thumb = nodeInfo.Prefab;
        nodeInfo.Textures = new PPTextureInfo[texs.Count];
        for (var i = 0; i < texs.Count; i++)
        {
            var t = texs[i];
            nodeInfo.Textures[i] = new PPTextureInfo()
            {
                Prefab = t.Name,
                EditorAngle = t.editor_angle,
                EditorPos = t.editor_pos
            };
            thumb += "-" + t.Name;
        }
        nodeInfo.Thumb = nodeInfo.IsStamp ? nodeInfo.Prefab + "_" + PTUtils.Md5Sum(thumb) : nodeInfo.Prefab;
        
        return new ThumbExportElement()
        {
            eulerAngle = angle,
            PrefabInfo = prefabInfo,
            BlockInfo = nodeInfo
        };
    }
}

[Serializable]
public class SkinInfo
{
    public string skin_name;
    public string skin_url;

    public bool EqualUrl(string url)
    {
        return skin_url.Equals(url);
    }
}

public class RenderDefine
{
    public static string ModelPrefixPath
    {
        get
        {
#if BLOCK_MODEL || BLOCK_EDITOR
            return "./res_models/models/".CreateDirIfNotExists();
#else
            return Application.persistentDataPath + "/res_models/";
#endif
        }
    }
    
    public static string TexPrefixPath
    {
        get
        {
#if BLOCK_MODEL || BLOCK_EDITOR
            return "./res_models/textures/".CreateDirIfNotExists();
#else
            return Application.persistentDataPath + "/res_models/";
#endif
        }
    }
}