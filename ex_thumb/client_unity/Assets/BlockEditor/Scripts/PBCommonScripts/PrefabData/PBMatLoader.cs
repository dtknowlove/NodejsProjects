using System.Collections.Generic;
using System.IO;
using PTGame.Core;
using UnityEngine;

public class PBMatLoader 
{
    private static Dictionary<string,Texture2D> blockTex = new Dictionary<string,Texture2D>();
    private static Dictionary<string, Material> blockMats = new Dictionary<string, Material>();

    private static string PrefixPath
    {
        get { return RenderDefine.TexPrefixPath; }
    }

    
    public static void Dispose()
    {
        blockTex.Values.ForEach(tex =>
        {
            if (tex != null)
                Object.Destroy(tex);
        });
        blockTex.Clear();
        foreach (var mat in blockMats)
        {
            if (mat.Value != null)
                Object.Destroy(mat.Value);
        }
        blockMats.Clear();
    }
    
    public static Material GetMaterial(MaterialInfo matInfo)
    {
        if (matInfo == null) return null;
        var mat = GetBaseMaterial(matInfo.shaderName);
        mat.name = matInfo.name;
        foreach (var item in matInfo.propsVector4)
        {
            mat.SetVector(item.name, item.data);
        }
        foreach (var item in matInfo.propsFloat)
        {
            mat.SetFloat(item.name, item.data);
        }

        foreach (var item in matInfo.propsTex)
        {
            Texture tex = LoadTexture(PrefixPath + Path.GetFileName(item.data));
            mat.SetTexture(item.name, tex);
        }
        return mat;
    }

    private static Material GetBaseMaterial(string shaderName)
    {
        var mat=new Material(GetCommonMaterial());
        mat.shader = Shader.Find(shaderName);
        return mat;
    }
    
    private static Material mCommonMaterial = null;
    private static Material GetCommonMaterial()
    {
        return mCommonMaterial ?? (mCommonMaterial = Resources.Load<Material>("mat_standard"));
    }

    private static Texture2D LoadTexture(string filePath)
    {
        Texture2D tex = null;
        if (!blockTex.ContainsKey(filePath))
        {
            byte[] fileData;

            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(2, 2,TextureFormat.RGBA32,false);
                tex.anisoLevel = 8;
                tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            }
            blockTex.Add(filePath,tex);
        }
        return blockTex[filePath];
    }

    private static string GetTextureTexPath(string textureName)
    {
        return string.Format(PrefixPath + "{0}.png",textureName);
    }

    private static string GetStickerTexPath(string textureName)
    {
        return string.Format(PrefixPath+ "{0}.png",textureName);
    }
    
    public static Material LoadTextureMatAsset(string matPath,string texture,bool isSticker)
    {
        var texPath = isSticker ? GetStickerTexPath(texture) : GetTextureTexPath(texture);
        var shaderName = isSticker ? "putaoshader/shader_textureunlit" : "Unlit/Transparent";
        Material mat = null;
        if (blockMats.ContainsKey(matPath))
        {
            mat = blockMats[matPath];
        }
        else
        {
            mat = new Material(GetCommonMaterial()) {enableInstancing = true};
            mat.shader = Shader.Find(shaderName);
            mat.mainTexture = LoadTexture(texPath);
            blockMats.Add(matPath, mat);
        }
        return mat;
    }
}
