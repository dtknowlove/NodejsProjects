using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Block.Editor;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PEFbx2GLTF : MonoBehaviour
{

    private const string FILENAME_RESCONFIG = "resconfig.json";

    [MenuItem("Assets/PutaoTool/Texture2Config", false)]
    public static void ExportTextures()
    {
        string[] dirs = {
            "Assets/BlockRes/CommonRes/Block_Textures",
            "Assets/BlockRes/Stickers/Sticker_Textures",
            "Assets/BlockRes/Textures/Texture_Textures"
        };
        
        List<string> textureFiles = new List<string>();

        foreach (var dir in dirs)
        {
            if (Directory.Exists(dir))
            {
                string[] files = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
                textureFiles.AddRange(files.Where(s =>
                {
                    return Path.GetExtension(s).ToLower() == ".png" || Path.GetExtension(s).ToLower() == ".jpg";
                } ).ToList());
            }
        }
        CreateTextureConfigs(textureFiles);
    }

    private static  void CreateTextureConfigs(List<string> files)
    {
        string texDir = "res_models";
        ResConfig resConfig = null;
        var databaseFile = "database_models_texture.txt";
        
        resConfig = new ResConfig(){items = new List<ResItem>()};
        
        foreach (var file in files)
        { 
            var refPath = Path.GetFullPath(file).Replace(Application.dataPath + "/", "").ToLower(); 
            var item = CreateConfigItem(Path.GetFullPath(file).ToLower(), Path.GetFileName(file),refPath);

            string destFile = Path.Combine(texDir, refPath);
            if (!Directory.Exists(Path.GetDirectoryName(destFile)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(destFile));
            }
            
            File.Copy(Path.GetFullPath(file),destFile,true);
           if (resConfig.items.FirstOrDefault(s => s.name == item.name)!=null)
           {
               EditorUtility.DisplayDialog("生成出错", "不可以存在相同的名称"+item.name, "确定");
           }

           resConfig.items.Add(item);
        }
        
        File.WriteAllText(databaseFile,JsonUtility.ToJson(resConfig,true));
        
        EditorUtility.DisplayDialog("生成所有Texture配置", "完成", "确定");
    }


    private static string dataPath;
    private const string title = @"fbx转换为gltf";
    
    [MenuItem("Assets/PutaoTool/Fbx2Gltf", false)]
    public static void ExportFbx2GLTF()
    {
        if (Selection.activeObject == null)
        {
            return;
        }
        
        if (!EditorUtility.IsPersistent(Selection.activeObject))
        {
            return;
        }
        
        dataPath = Application.dataPath;
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
		
        List<string> fbxFiles = new List<string>();
        if (Directory.Exists(path))
        {
            string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            fbxFiles.AddRange(files.Where(s => Path.GetExtension(s).ToLower() == ".fbx").ToList());
           
        }
        else
        {
            foreach (var item in Selection.gameObjects)
            {
                string p = AssetDatabase.GetAssetPath(item);
                if (Path.GetExtension(p).ToLower() == ".fbx")
                {
                    fbxFiles.Add(p);
                  
                }
            }
        }
        
        if (fbxFiles.Count == 0)
        {
            EditorUtility.DisplayDialog(title, "该目录下没有fbx文件", "确定");
        }
        else
        {
            ChangeFormat(fbxFiles);
        }
    }
    
    
    private static void ChangeFormat(List<string> fbxFiles)
    {
        ResConfig resConfig = null;
        var databaseFile = "database_models_gltf.txt";
        if (!File.Exists(databaseFile))
        {
            resConfig = new ResConfig(){items = new List<ResItem>()};
            
        }
        else
        {
            var content = File.ReadAllText(databaseFile);
            resConfig = JsonUtility.FromJson<ResConfig>(content);
        }
        
        bool hasError = false;
        StringBuilder errorMsg = new StringBuilder();
        string destDir = "res_models";
       
        int totalCount = fbxFiles.Count;
        int counter = 0;
        foreach (var fbxFile in fbxFiles)
        {
            string input = Path.GetFullPath(fbxFile);

            string refPath = Path.GetFullPath(fbxFile).Replace(dataPath + Path.DirectorySeparatorChar, "").ToLower();
            string outputDir = Path.Combine(destDir,refPath);
            outputDir = Path.GetDirectoryName(outputDir)+Path.DirectorySeparatorChar+Path.GetFileNameWithoutExtension(fbxFile);
            outputDir = outputDir.ToLower();
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }
            
            var glftFileName = Path.GetFileNameWithoutExtension(fbxFile) + ".gltf";
            var gltfFilePath = Path.Combine(Environment.CurrentDirectory,Path.Combine(outputDir,glftFileName).ToLower());
            
            string arguments = input + " -o " + gltfFilePath;
            RunCommand(arguments);
            
            EditorUtility.DisplayProgressBar(title,"请稍后",counter/totalCount*1.0f);
            counter++;

            var prefix = outputDir.Replace("res_models/", string.Empty);
            var bufferFilePath = prefix + "/buffer.bin";
            var gltfRefPath = prefix + "/" + glftFileName;
            
            var realPath = outputDir + "/buffer.bin";
            var gltfRealPath =  outputDir  + "/" + glftFileName;
            
            
            var gltfItems = resConfig.items.Where(s => s.path == gltfRefPath);
            var bufferItems = resConfig.items.Where(s => s.path == bufferFilePath);
            
            for (int i=0;i<gltfItems.Count();i++)
            {
                resConfig.items.Remove(gltfItems.ElementAt(i));
            }
            
            for (int i=0;i<bufferItems.Count();i++)
            {
                resConfig.items.Remove(bufferItems.ElementAt(i));
            }

            var gltfItem = CreateConfigItem(gltfRealPath, glftFileName, gltfRefPath);
            var bufferItem = CreateConfigItem(realPath, Path.GetFileNameWithoutExtension(fbxFile)+"/buffer.bin", bufferFilePath);
            resConfig.items.Add(gltfItem);
            resConfig.items.Add(bufferItem);

        }
        EditorUtility.ClearProgressBar();

        if (hasError)
        {
            Debug.LogError(errorMsg.ToString());
        }
        
        File.WriteAllText(databaseFile,JsonUtility.ToJson(resConfig,true));
        
        EditorUtility.DisplayDialog(title, hasError?errorMsg.ToString():"转换完成", "确定");
    }


    private static void RunCommand(string command)
    {
        Process process = new Process();
        string cmd = dataPath + "/Plugins/FBX2glTF-darwin-x64";
        process.StartInfo.FileName = cmd;
        process.StartInfo.Arguments = command;

        process.StartInfo.CreateNoWindow = false; // 获取或设置指示是否在新窗口中启动该进程的值（不想弹出powershell窗口看执行过程的话，就=true）
        process.StartInfo.ErrorDialog = true; // 该值指示不能启动进程时是否向用户显示错误对话框
        process.StartInfo.UseShellExecute = false;

        process.Start();

        process.WaitForExit();
        process.Close();
        
        
    }
    
    private static ResItem CreateConfigItem(string realPath, string fileName, string refPath)
    {
        ResItem resItem = new ResItem {name = fileName, path = refPath};
        byte[] fileBytes = GetFileBytes(realPath);
        resItem.hash = GetMD5(fileBytes);
        resItem.size = fileBytes.Length;
        return resItem;
    }
    
    private static byte[] GetFileBytes(string filePath)
    {
        FileStream fileStream = new FileStream(filePath, FileMode.Open);
        int num = (int) fileStream.Length;
        byte[] array = new byte[num];
        fileStream.Read(array, 0, num);
        fileStream.Close();
        return array;
    }
    
    private static string GetMD5(byte[] data)
    {
        MD5 mD = new MD5CryptoServiceProvider();
        byte[] array = mD.ComputeHash(data);
        string text = "";
        byte[] array2 = array;
        foreach (byte value in array2)
        {
            text += Convert.ToString(value, 16);
        }

        return text;
    }
    
    [System.Serializable]
    public class ResConfig
    {
        public string resversion = "";

        public List<ResItem> items;
    }

    [System.Serializable]
    public class ResItem
    {
        public string name = "";

        public string path = "";

        public string hash = "";

        public float size = 0f;
    }
}