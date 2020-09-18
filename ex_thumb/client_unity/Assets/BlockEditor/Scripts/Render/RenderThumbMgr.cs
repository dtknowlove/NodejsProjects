using System;
using System.IO;
using Putao.PaiBloks.Common;
using UniRx;
using UnityEditor;
using UnityEngine;

public class RenderThumbMgr : MonoBehaviour
{
    public const string Url = "http://127.0.0.1:8081/done?";
    public const string Url_SegDot = "http://127.0.0.1:8081/segdot?";
    public static string Key_DataFile
    {
        get
        {
            var projectPath = Path.GetFullPath(Application.dataPath + "/..");
            return projectPath.Substring(projectPath.LastIndexOf("/")+1) + "/Key_DataFile";
        }
    }

    private static string Prefix = "https://file-oss.putaocdn.com/blocks/resources/mold/low/";
    private static string stickerModelPrefix = "https://file-oss.putaocdn.com/blocks/resources/stickers/";
    private static string texModelPrefix = "https://file-oss.putaocdn.com/blocks/resources/textures/";
    private static string texPrefix = "https://apk-download.putaocdn.com/data_buluke/version_test/res_models/textures/";
    [MenuItem("BlockTool/Render")]
    public static void Test()
    {
        var config = "/Users/admin/Desktop/tar_config/configs/config_22007_01_05.txt";
        var blockConfig = PBBlockConfigManager.LoadBlockInfosWithoutAnim(config);

        PPPrefabInfo targetPrefab = null;
        PPBlockInfo targetBlock = null;
        foreach (PBPartInfo partInfo in blockConfig.BlockInfos)
        {
            if (partInfo.BlockInfo.IsSticker && partInfo.PrefabName.Equals("sticker_prefab_ppbbbmhmf_09_common"))
            {
                targetBlock = partInfo.BlockInfo;
                break;
            }
        }
        
        foreach (var t in blockConfig.PrefabInfos)
        {
            if (t.IsSticker && t.Name.Equals("sticker_prefab_ppbbbmhmf_09_common"))
            {
                targetPrefab = t;
                break;
            }
        }

        if (targetPrefab == null || targetBlock == null)
        {
            Debug.LogError("空");
            return;
        }

        if (!string.IsNullOrEmpty(targetPrefab.Material))
        {
            targetPrefab.MaterialInfos.Add(PBDataBaseManager.Instance.GetMatInfoByName(targetPrefab.Material));
        }
        targetPrefab.Texs.ForEach(t =>
        {
            t.skin_url = texPrefix + t.Texture + ".png";
            t.bin_url = texModelPrefix + t.Model + "/buffer.bin";
            t.gltf_url = texModelPrefix + t.Model + "/" + t.Model + ".gltf";
        });

        var renderinfo = new RenderInfo();
        renderinfo.scale = 2;
        renderinfo.items = new[]
        {
            new RenderItem()
            {
                name = targetPrefab.Name,
                model = targetPrefab.Model,
                material = targetPrefab.Material,
                matinfos = targetPrefab.MaterialInfos,
                bin_url = stickerModelPrefix + targetPrefab.Model + "/buffer.bin",
                gltf_url = stickerModelPrefix + targetPrefab.Model + "/" + targetPrefab.Model + ".gltf",
                skins = !string.IsNullOrEmpty(targetPrefab.Texture)? new[]
                {
                    new SkinInfo()
                    {
                        skin_name = targetPrefab.Texture,
                        skin_url = texPrefix + targetPrefab.Texture + ".png",
                    }
                }:new SkinInfo [0],
                texs = targetPrefab.Texs
            }
        };
        File.WriteAllText(Application.dataPath+"/test111.json", JsonUtility.ToJson(renderinfo, true));
        Debug.Log("===>>完成");
    }
    
    [MenuItem("BlockTool/Upload")]
    public static void TestUpload()
    {
        var filePath = SavePath + "/35/sticker_prefab_ppbbbmhmf_10_common.png";
        UploadUtility.UploadFile(filePath, (res,msg) =>
        {
            Debug.Log(UploadUtility.GetDownloadFileUrl(res));
        });
    }

    private static string SavePath
    {
        get { return Path.GetFullPath(string.Format("./../renderserver/public{0}", mRenderFile == null ? string.Empty : "/" + mRenderFile.id)); }
    }

    private static RenderFile mRenderFile = null;
    private string mMsg = string.Empty;

    void Start()
    {
        ReportSegTime(3, "ToDownload");
        var key = PlayerPrefs.GetString(Key_DataFile, String.Empty);
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogError("===>>>启动Unity传入参数不能为空");
            Quit();
            return;
        }
        var filepath = Path.GetFullPath(string.Format("./../renderserver/cache/{0}.json", key));
        Debug.Log(filepath);
        mRenderFile = JsonUtility.FromJson<RenderFile>(File.ReadAllText(filepath));
        if (mRenderFile == null)
        {
            Debug.LogError("===>>>渲染数据不能为空");
            Quit();
            return;
        }
        mMsg = string.Empty;
        RenderDownloader.Execute(mRenderFile, (success) =>
        {
            if (success)
            {
                ReportSegTime(4, "ToRender");
                Debug.Log("====>>>下载零件数据完成,开始渲染..");
                Render(mRenderFile.info,(files,msg)=>
                {
                    if (!string.IsNullOrEmpty(msg))
                        mMsg = msg;
                    OnFinish(files);
                });
            }
            else
            {
                mMsg = "下载零件数据失败";
                Debug.LogError("===>>>" + mMsg);
                OnFinish(null);
            }
        });
    }

    private void OnFinish(UploadFiles mfiles)
    {
        //request to node finish
        if (mRenderFile == null)
            return;
        var url = string.Format("{0}id={1}&msg={2}", Url, mRenderFile.id, mMsg);
        if (mfiles != null)
        {
            var dataPath = string.Format("{0}/{1}.json", SavePath, mRenderFile.id);
            File.WriteAllText(dataPath, JsonUtility.ToJson(mfiles));
        }
        ReportSegTime(6, "Done");
        ObservableWWW.Get(url).Subscribe(res =>
        {
            Debug.Log(res);
            Quit();
        });
        //clear
        PlayerPrefs.DeleteKey(Key_DataFile);
    }

    private void Quit()
    {
        EditorApplication.isPlaying = false;
        EditorApplication.Exit(1);
    }

    public static void Render(RenderInfo renderInfo, Action<UploadFiles,string> callBack = null)
    {
        var info = new ThumbExportInfo();
        info.thumbs = renderInfo.GetExportItems().ToArray();
        info.scaleFactor = renderInfo.scale;
        info.saveFolder = SavePath;

        PEThumbExporter exporter = GameObject.FindObjectOfType<PEThumbExporter>();
        exporter.SetExportInfo(info);

        GameObject.FindObjectOfType<PEThumbExporter>().StartRender(callBack);
    }


    private static int GetID
    {
        get
        {
            var key = PlayerPrefs.GetString(Key_DataFile, String.Empty);
            if (string.IsNullOrEmpty(key))
                return -1;
            var filepath = Path.GetFullPath(string.Format("./../renderserver/cache/{0}.json", key));
            var mRenderFile = JsonUtility.FromJson<RenderFile>(File.ReadAllText(filepath));
            if (mRenderFile == null)
                return -1;
            return mRenderFile.id;
        }
    }

    public static void ReportSegTime(int segid, string segdesc)
    {
        var url = string.Format("{0}id={1}&segid={2}&segdesc={3}", Url_SegDot, GetID, segid, segdesc);
        ObservableWWW.Get(url).Subscribe(Debug.Log);
    }
}