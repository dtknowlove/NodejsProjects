using System;
using System.Collections.Generic;
using System.IO;
using PTGame.Core;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

public class PPBlockLoader
{
    private Dictionary<string, Material> blockMats = new Dictionary<string, Material>();
    private List<GameObject> blockObj = new List<GameObject>();
    private Dictionary<string, AssetBundle> blockAbs = new Dictionary<string, AssetBundle>();
    
    private static string PrefixPath
    {
        get { return RenderDefine.ModelPrefixPath; }
    }

    public void Dispose()
    {
        blockObj.ForEach(t =>
        {
            if (t != null)
                Object.Destroy(t);
        });
        blockObj.Clear();
        PBMatLoader.Dispose();
        foreach (var mat in blockMats)
        {
            if (mat.Value != null)
                Object.Destroy(mat.Value);
        }
        blockMats.Clear();
        blockAbs.Clear();
        PTGLTFLoader.Dispose();
    }
    
    public GameObject CreateBlock(PPPrefabInfo info, Action loadFinish = null)
    {
        if (info == null)
        {
            throw new ArgumentNullException("PPPrefabInfo can't be null!");
        }
        var prefabName = info.Name;
        if (info.IsSticker)
        {
            return CreateSticker(info, loadFinish);
        }

        var model = info.Model;
        var modelPath = Path.Combine(PrefixPath, model + "/" + model + ".gltf");

        //load gltf
        GameObject block = null;
        block = PTGLTFLoader.loadGltf(modelPath, () =>
        {
            var blockRoot = block.transform.Find("Root Scene");
            if (blockRoot.GetComponentInChildren<Renderer>(true) != null)
            {
                AssignMaterials(blockRoot.gameObject, info.Material, info.MaterialInfos);
            }
            else
            {
                PTDebug.LogError("Block:<color=#FF00FF>{0}</color> load failed! Model:<color=#FF00FF>{1}</color>", prefabName, model);
            }
            loadFinish.InvokeGracefully();
        },OnInitializeGltfObject);
        blockObj.Add(block);

        block.name = model;

        Animator animator = block.GetComponent<Animator>();
        if (animator != null)
        {
            GameObject.DestroyImmediate(animator);
        }
        return block;
    }


    public GameObject CreateSticker(PPPrefabInfo info, Action loadFinish = null)
    {
        if (info == null)
        {
            throw new ArgumentNullException("PPPrefabInfo can't be null!");
        }
        string matPath = Path.Combine(AppBlockPath(BlockPath.Sticker_Material_Dir), info.Name);
        var modelPath = Path.Combine(PrefixPath, info.Model + "/" + info.Model + ".gltf");
        GameObject sticker = null;
        sticker = PTGLTFLoader.loadGltf(modelPath, () =>
        {
            var stickerRoot = sticker.transform.Find("Root Scene");
            var render = stickerRoot == null ? null : stickerRoot.GetComponentInChildren<Renderer>(true);
            if (render != null)
            {
                Material mat = PBMatLoader.LoadTextureMatAsset(matPath, info.Texture, true);
                render.receiveShadows = false;
                render.shadowCastingMode = ShadowCastingMode.Off;
                render.sharedMaterial = mat;
            }
            else
            {
                PTDebug.LogError("Sticker:<color=#FF00FF>{0}</color> load failed! Model:<color=#FF00FF>{1}</color>", info.Name, info.Model);
            }
            loadFinish.InvokeGracefully();
        },OnInitializeGltfObject);
        blockObj.Add(sticker);

        return sticker;
    }


    public GameObject CreateTexture(PPPrefabTexInfo info, Action loadFinish = null)
    {
        if (info == null)
        {
            throw new ArgumentNullException("PPPrefabInfo can't be null!");
        }
        var prefabName = info.Name;
        string matPath = Path.Combine(AppBlockPath(BlockPath.Texture_Material_Dir), prefabName);

        var model = info.Model;
        var modelPath = Path.Combine(AppBlockPath(BlockPath.Texture_Fbx_Dir), model);
        modelPath = modelPath + "/" + model + ".gltf";

        GameObject texObj = null;
        texObj = PTGLTFLoader.loadGltf(modelPath, () =>
        {
            var texRoot = texObj.transform.Find("Root Scene");
            var render = texRoot == null ? null : texRoot.GetComponentInChildren<Renderer>(true);
            if (render != null)
            {
                Material mat = PBMatLoader.LoadTextureMatAsset(matPath, info.Texture, false);
                render.receiveShadows = false;
                render.shadowCastingMode = ShadowCastingMode.Off;
                render.material = mat;
            }
            else
            {
                PTDebug.LogError("Texture:<color=#FF00FF>{0}</color> load failed! Model:<color=#FF00FF>{1}</color>", prefabName, model);
            }
            loadFinish.InvokeGracefully();
        },OnInitializeGltfObject);

        texObj.name = prefabName;
        blockObj.Add(texObj);

        return texObj;
    }

    private void OnInitializeGltfObject(GameObject rootScene)
    {
        if (rootScene == null)
            return;
        var createdObjs = rootScene.GetComponentsInChildren<Renderer>();
        if (createdObjs.Length == 1)
            createdObjs.ForEach(t => t?.Identity());
    }

    private string AppBlockPath(string assetPath)
    {
        #if BLOCK_MODEL
        return Path.Combine("./",Path.Combine("res_models",assetPath.Substring(7).ToLower()));
        #else
        return Path.Combine(Application.persistentDataPath,Path.Combine("res_models",assetPath.Substring(7).ToLower()));
        #endif
    }

    enum MaterialType
    {
        Default = 0,
        SingleMeshMultiMat = 1,
    }
    
    private void AssignMaterials(GameObject block, string materialLabel,List<MaterialInfo> matInfos)
    {
        var matType = MaterialType.Default;
        var singleMaterials = new Dictionary<string, MaterialInfo>();
        var multiMaterials = new List<MaterialInfo>();
        
        if (materialLabel.IndexOf(":") < 0)
        {
            Renderer renderer = block.GetComponentInChildren<Renderer>(true);
            singleMaterials.Add(renderer.gameObject.name, matInfos[0]);
        }
        else
        {
            MaterialType type = MaterialType.Default;

            string[] materials = materialLabel.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < materials.Length; i++)
            {
                string[] element = materials[i].Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries);
                if (element[0].Equals("type"))
                {
                    type = (MaterialType) int.Parse(element[1]);
                }
                else
                {
                    var index = Mathf.Max(0, i - 1);
                    if (type == MaterialType.Default)
                        singleMaterials.Add(element[0], matInfos[index]);
                    else
                        multiMaterials.Add(matInfos[index]);
                }
            }
        }
       
        if (singleMaterials.Count > 0)
            AssignSingleMaterials(block, singleMaterials);
        if (multiMaterials.Count > 0)
            AssignMultiMaterials(block, multiMaterials);
    }

    private void AssignSingleMaterials(GameObject block, Dictionary<string, MaterialInfo> materials)
    {
        Renderer[] renders = block.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renders.Length; i++)
        {
            Renderer render = renders[i];
            render.receiveShadows = false;
            render.shadowCastingMode = ShadowCastingMode.On;
            MaterialInfo materialStr;
            if (!materials.TryGetValue(render.gameObject.name, out materialStr))
                continue;
            
            Material mat = LoadCommonMatAsset(materialStr);
            render.sharedMaterial = mat;
        }
    }

    private void AssignMultiMaterials(GameObject block, List<MaterialInfo> materials)
    {
        Renderer render = block.GetComponentInChildren<Renderer>(true);
        render.receiveShadows = false;
        render.shadowCastingMode = ShadowCastingMode.On;

        int index = 0;
        Material[] mats = render.sharedMaterials;
        foreach (var materialStr in materials)
        {
            if (mats.Length <= index)
                break;

            Material mat = LoadCommonMatAsset(materialStr);
            mats[index] = mat;
            index++;
        }
        render.sharedMaterials = mats;
    }

    private Material LoadCommonMatAsset(MaterialInfo matInfo)
    {
        //model 工程中 所有的 Material 必须放到 commonres里面，不能放到对应的category里面，所有的material不能重名
        string matPath = Path.Combine(BlockPath.MaterialCommon(), matInfo.name);

        Material mat = null;
        if (blockMats.ContainsKey(matPath))
        {
            mat = blockMats[matPath];
        }
        else
        {
            mat = PBMatLoader.GetMaterial(matInfo);
            blockMats[matPath] = mat;
        }
        return mat;
    }
}