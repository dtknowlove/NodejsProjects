using System.Collections;
using System.Collections.Generic;
using Putao.BlockRes;
using UnityEngine;

public class TestResUpdator : MonoBehaviour
{
    private void Start()
    {
        ResUpdateInfo info = new ResUpdateInfo();
        info.CategoryId = 1;
        info.PrefabList = new[]
        {
            "category_1/block_prefabs/paiblock_feat_signboard_light_purple",
        };
        
        info.ThumbList = new []
        {
            "category_1/block_thumbs/paiblock_feat_signboard_light_purple"
        };

        PTBlockResUpdator updator = gameObject.AddComponent<PTBlockResUpdator>();
        updator.UpdateRes(info);
    }
}
