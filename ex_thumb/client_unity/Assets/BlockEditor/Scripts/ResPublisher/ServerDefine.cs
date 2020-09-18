using PTGame;
using PTGame.Core;
using UnityEngine;

namespace Putao.GameCommon
{
    
    public class ServerDefine
    {
        public static bool isAndroidPlatform = false;
        #region 远程版本号获取相关
        
        [System.Serializable]
        private class ResRemoteData
        {
            public int code;
            public ResRemoteItemData data;
            public string message;
        }
        
        [System.Serializable]
        private class ResRemoteItemData
        {
            public string app_version;
            public string appid;
            public string unity_version;
        }

        private static string ResVersionPrefix
        {
            get
            {
                var result = "https://api-blocks.putao.com/portal/versions";
                switch (PTUGame.Instance.Runtime)
                {
                    case PTRuntime.ONLINE:
                        return result;
                    case PTRuntime.DEV:
                    case PTRuntime.TEST:
                        return "http://test-api-blocks.ptdev.cn/portal/versions";
                }
                return result;
            }
        }

        public static string ResVersionUrl
        {
            get
            {
                var appid = "8311";
#if UNITY_IOS
                appid = "8312";
#endif
                return string.Format(ResVersionPrefix + "?app_version={0}&appid={1}", PTUniInterface.GetAppVersion(), appid);
            }
        }

        public static void SetResVersion(string text)
        {
            var resdata = JsonUtility.FromJson<ResRemoteData>(text);
            mResVersion = resdata.data.unity_version;
            PTDebug.Log("当前远程资源版本号:{0}", mResVersion);
        }

        #endregion

        public class ResUrl
        {
            public string remoteResPath;
            public string localResPath;
            public string remoteConfigPath;
            public string localConfigPath;
        }

        public static string mResVersion = "526";

        private static string GetServerUrl()
        {
            //内网和外网使用同一个ftp服务器
            var serverUrl = "https://apk-download.putaocdn.com/game_buluke";
//            if (PTUGame.Instance.Runtime == PTRuntime.ONLINE)
//            {
//                serverUrl = "https://apk-download.putaocdn.com/game_buluke";
//            }
            return serverUrl;
        }

        public static ResUrl GetBlockBotsResUrl()
        {
            var isAndroid = isAndroidPlatform;
            var server = "https://apk-download.putaocdn.com/game_buluke/blockbots/version_{0}/{1}";
            var appVersion = Application.version.Replace(".", "").Trim();
            var serverUrl = string.Format(server,appVersion,isAndroid?"android":"ios");
       
            var remotePath = serverUrl+"/resconfig.json";

            #if UNITY_IOS
            var localPath = Application.persistentDataPath + "/AssetBundles/iOS/AB_BlockBots_iOS/resconfig.json";
            #else 
            var localPath = Application.persistentDataPath + "/AssetBundles/Android/AB_BlockBots_Android/resconfig.json";
            #endif

            return new ResUrl() {remoteConfigPath = remotePath, localConfigPath = localPath};
        }

        public static ResUrl GetConfigUrl(string dirName, string fileName)
        {
            var serverUrl = GetServerUrl();

            var version = mResVersion;
            
            var remotePath = serverUrl + "/version_" + version + "/" + dirName + "/" + fileName;

            var localPath = Application.persistentDataPath + "/" + dirName + "/" + fileName;

            return new ResUrl() {remoteConfigPath = remotePath, localConfigPath = localPath};
        }
        
        public static ResUrl GetThumbUrl(string configfile)
        {
            var serverUrl = GetServerUrl();

            var version = mResVersion;

            var remotePath = serverUrl + "/version_" + version + "/" + "block_thumbs_hash";

            var localPath = Application.persistentDataPath + "/" + "block_thumbs_hash";
            
            var remoteConfigPath = serverUrl + "/version_" + version + "/" + "config_paibloks_buildanim_thumbs"+"/"+configfile;
            
            var localConfigPath = Application.persistentDataPath + "/" + "config_paibloks_buildanim_thumbs"+"/"+configfile;

            return new ResUrl() {remoteResPath = remotePath, localResPath = localPath,
                remoteConfigPath = remoteConfigPath,localConfigPath = localConfigPath};
        }

        public static ResUrl GetBlockModeRes(string configfile,bool isSku = false)
        {
            var isAndroid = isAndroidPlatform;
            
            var serverUrl = GetServerUrl();
            
            var version = mResVersion;
            
            var remotePath = serverUrl + "/version_" + version;

            var localPath = Application.persistentDataPath;

            var dir = isSku ? "config_" : "config_buildanim_";
            
            var remoteConfigPath = string.Format(serverUrl + "/version_" + version + "/" + dir+"{0}"+"/"+configfile,isAndroid?"modelres_android":"modelres_ios");
            
            var localConfigPath = string.Format(Application.persistentDataPath + "/" + dir+"{0}"+"/"+configfile,isAndroid?"modelres_android":"modelres_ios");

            return new ResUrl() {remoteResPath = remotePath, localResPath = localPath,
                remoteConfigPath = remoteConfigPath,localConfigPath = localConfigPath};
        }


        #region noova 孩子数据同步

        public class SyncDataUrl
        {
            public string url_savedatainfo;
            public string url_getdatainfo;
            public string url_getuploadtoken;
            
            public string url_uploadfile = "https://upload.putaocloud.com/largeupload";//上传文件地址
            public string url_uploadfileinfo = "https://upload.putaocloud.com/upfileinfo";//断点续传获取信息
            public string url_downloadfile = "https://mall-file.putaocdn.com/largefile";//下载文件地址
        }

        public static SyncDataUrl GetSyncDataUrl()
        {
            var syncDataUrl = new SyncDataUrl();
            if (PTUGame.Instance.Runtime == PTRuntime.ONLINE)
            {
                syncDataUrl.url_savedatainfo = "https://api-blocks.putao.com/relation/resource";
                syncDataUrl.url_getdatainfo = "https://api-blocks.putao.com/relation/resource";
                syncDataUrl.url_getuploadtoken = "https://api-blocks.putao.com/upload/token";
            }
            else
            {
                syncDataUrl.url_savedatainfo = "http://test-api-blocks.ptdev.cn/relation/resource";
                syncDataUrl.url_getdatainfo = "http://test-api-blocks.ptdev.cn/relation/resource";
                syncDataUrl.url_getuploadtoken = "http://test-api-blocks.ptdev.cn/upload/token";
            }
            return syncDataUrl;
        }
       

        #endregion
      
    }
}
