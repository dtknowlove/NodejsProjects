#if BLOCK_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;

public class PEBlockLoader
{
      public static GameObject CreateBlock(string prefabName, PolygonType polygonType)
      {
            Category category = PBDataBaseManager.Instance.GetCategoryWithPrefabName(prefabName);
            string dirPath = BlockPath.Prefab(category, polygonType);
            string prefabPath = dirPath + "/" + prefabName + ".prefab";
      
            if (!File.Exists(prefabPath))
            {
                  if (prefabName.StartsWith("sticker_"))
                  {
                        PEPrefabGeneratorUtil.CreateStickerPrefab(prefabName);
                  }
                  else
                  {
                        BlockData blockData = PBDataBaseManager.Instance.GetBlockWithPrefabName(prefabName);
                        PEPrefabGeneratorUtil.CreateSinglePrefab(prefabName, blockData.model, blockData.material, blockData.material_high, category, polygonType);
                  }
            }
      
            GameObject tobj = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (tobj == null)
                  Debug.LogError(">>>>找不到prefab：" + prefabPath);
            GameObject gameObj = GameObject.Instantiate(tobj);
            gameObj.AddComponent<PEBlockAlign>().SetRefPointTypes();
            var boxCollider = gameObj.AddComponent<BoxCollider>();
      
            if (prefabName.StartsWith("sticker"))
            {
                  boxCollider.size = boxCollider.size + Vector3.up * 0.3f;
            }
      
            return gameObj;
      }
      
      public static GameObject CreateTexture(string prefabName)
      {
            string texPath = Path.Combine(BlockPath.Texture_Prefab_Dir, prefabName + ".prefab");
            if (!File.Exists(texPath))
            {
                  PEPrefabGeneratorUtil.CreateTexturePrefab(prefabName);
            }
      
            GameObject texPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(texPath);
            if (texPrefab == null)
                  Debug.LogError(">>>>找不到texture prefab：" + texPath);
            GameObject texObj = GameObject.Instantiate(texPrefab);
            texObj.AddComponent<PEBlockAlign>().SetRefPointTypes();
            texObj.AddComponent<BoxCollider>();
      
            return texObj;
      }
}

#endif