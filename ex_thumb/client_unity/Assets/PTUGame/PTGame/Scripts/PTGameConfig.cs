using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters;
using PTGame.Core;
using UnityEngine;
using UnityEngine.Networking;

namespace  PTGame
{
    ///
    /// http://10.1.223.240:5016/ptgameenv
    /// 
    /// <summary>
    /// 需要跟PTBuild 插件中的值保持一致 ,不要随意更改
    /// </summary>
    public enum PTRuntime
    {
        ONLINE = 0, //线上服
        DEV = 1,//开发服
        TEST = 2//测试服
    }
    
    
    public class PTGameConfig
    {
        public class GameConfig
        {
            public string chanelName;

            public string custom = "ptgame";
        }

        public class ConfigData
        {
            public PTRuntime runtime = PTRuntime.ONLINE;
            public string chanelName ="putao";
            public GameConfig gameConfig;
        }

        private class EnvResult {

            public int type = 0;
            public int code = -1;
            public string msg = "";
        }
 
       
        
        private const string KeyPtgameServerenvType = "KEY_PTGAME_SERVERENV_TYPE";
        
        #if UNITY_EDITOR
        

        private const string NAME_MENU_ONLINE = "PuTaoTool/PTGame/ServerEnv/Online";
        private const string NAME_MENU_TEST = "PuTaoTool/PTGame/ServerEnv/Test";
        private const string NAME_MENU_DEV = "PuTaoTool/PTGame/ServerEnv/Dev";
	
        [UnityEditor.MenuItem(NAME_MENU_ONLINE)]
        private static void SetEnvOnline()
        {
           UnityEditor.EditorPrefs.SetInt(KeyPtgameServerenvType, 0);
            SetChecked();
        }
        
        [UnityEditor.MenuItem(NAME_MENU_ONLINE,true)]
        private static bool SetEnvOnlineValidate()
        {
            SetChecked();
            return true;
        }

        [UnityEditor.MenuItem(NAME_MENU_TEST)]
        private static void SetEnvTest()
        {
            UnityEditor.EditorPrefs.SetInt(KeyPtgameServerenvType, 2);
            SetChecked();
        }
        
        [UnityEditor.MenuItem(NAME_MENU_TEST,true)]
        private static bool SetEnvTestValidate()
        {
            SetChecked();
            return true;
        }
        
   
        [UnityEditor.MenuItem(NAME_MENU_DEV)]
        private static void SetEnvDev()
        {
            UnityEditor.EditorPrefs.SetInt(KeyPtgameServerenvType, 1);
            SetChecked();
        }
        [UnityEditor.MenuItem(NAME_MENU_DEV,true)]
        private static bool SetEnvDevValidate()
        {
            SetChecked();
            return true;
        }
        
        private static void SetChecked()
        {
            int type =  UnityEditor.EditorPrefs.GetInt(KeyPtgameServerenvType, 0);
            UnityEditor.Menu.SetChecked(NAME_MENU_ONLINE, type ==0);
            UnityEditor.Menu.SetChecked(NAME_MENU_TEST, type ==2);
            UnityEditor.Menu.SetChecked(NAME_MENU_DEV, type ==1);
        }
        
        #endif

       


        public static ConfigData GetConfigData()
        {
            var ptgameConfig = LoadGameConfig();
            
            #if UNITY_EDITOR
            
            int type =  UnityEditor.EditorPrefs.GetInt(KeyPtgameServerenvType, 0);
            
            return  new ConfigData(){runtime = (PTRuntime)type,gameConfig = ptgameConfig};
            
            #endif

    
            var configData = new ConfigData() {chanelName = ptgameConfig.chanelName, gameConfig = ptgameConfig};
            
         
            if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
            {
                string parameters = string.Format("appname={0}&bundleid={1}&version={2}&buildnum={3}&custom={4}",
                   Application.productName, Application.identifier, Application.version, PTUniInterface.GetBuildNumOrVersionCode(), ptgameConfig.custom);
                EnvResult envResult = null;
     
                RequestSync("http://10.1.223.240:5016/ptgameenv/GetEnvType?" + parameters, 1, (result, msg) =>
                {
                        if (result == 0)
                        {
                            Debug.LogError(msg+">>>>");
                            envResult = JsonUtility.FromJson<EnvResult>(msg);
                            if (envResult != null)
                            {
                                configData.runtime = (PTRuntime)envResult.type;
                            }
                        }
                });
            }
            return configData;
        }
        
        private static void RequestSync (string url, int timeout, Action<int,string> callback)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            request.timeout = timeout;
            request.SendWebRequest();

            while (!request.isDone){ }
			
            if (request.error != null||string.IsNullOrEmpty(request.downloadHandler.text))
            {
                callback.InvokeGracefully(-1, request.error);

            } else {
				
                callback.InvokeGracefully(0, request.downloadHandler.text);
				
            }
            request.Dispose();
        }

        private static GameConfig LoadGameConfig()
        {
            string url = Application.streamingAssetsPath+"/ptgameconfig.json";
            
            if (Application.platform == RuntimePlatform.IPhonePlayer||
                Application.platform == RuntimePlatform.OSXEditor||
                Application.platform == RuntimePlatform.WindowsEditor)
            {
                if (!File.Exists(url))
                {
                    return new GameConfig();
                }
                
                url = "file://" + url;
            }

            GameConfig gameConfig = null;
            
            RequestSync(url, 1, (result, content) =>
            {
                if (result == 0)
                {
                     gameConfig = JsonUtility.FromJson<GameConfig>(content);
                }
                else
                {
                    gameConfig = new GameConfig();
                }
            });

            return  gameConfig;
        }
    }
    
  
}

