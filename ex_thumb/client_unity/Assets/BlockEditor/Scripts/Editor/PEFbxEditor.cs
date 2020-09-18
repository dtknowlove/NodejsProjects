//using System;
//using System.Collections.Generic;
//using System.IO;
//using LitJson;
//using UnityEditor;
//using UnityEngine;
//
//public class PEFbxEditor
//{
//    public class FBXError
//    {
//        public string fbxName;
//
//        public bool scaleError;
//        public bool angleError;
//        public string fileScale; //3dmax export unit is not 1cm
//        public string scaleFactor; //unity modelimporter scaleFactor
//
//        public bool isError()
//        {
//            return scaleError || angleError || !fileScale.Equals("0.010") || !scaleFactor.Equals("1.000");
//        }
//    }
//
//    [MenuItem("BlockTool/FBX/Export Checklist", false, GlobalDefine.Menu_FBX)]
//    public static void ExportCheckList()
//    {
//        List<string> fbxs = new List<string>();
//			
//        string[] directories = Directory.GetDirectories(Application.dataPath + "/BlockRes", "Block_Fbxs", SearchOption.AllDirectories);
//        for (int i = 0; i < directories.Length; i++)
//        {
//            string[] files = Directory.GetFiles(directories[i], "*.FBX", SearchOption.AllDirectories);
//            fbxs.AddRange(files);
//            files = Directory.GetFiles(directories[i], "*.fbx", SearchOption.AllDirectories);
//            fbxs.AddRange(files);
//        }
//
//        try
//        {
//            List<FBXError> errors = new List<FBXError>();
//            
//            for (int i = 0; i < fbxs.Count; i++)
//            {
//                string p = fbxs[i].Substring(fbxs[i].IndexOf("Assets"));
//                EditorUtility.DisplayProgressBar("检查FBX格式", p, (float) i / fbxs.Count);
//
//                FBXError error = new FBXError();
//                ModelImporter mi = AssetImporter.GetAtPath(p) as ModelImporter;
////                error.fileScale = mi.fileScale.ToString("0.000");
//                error.scaleFactor = mi.globalScale.ToString("0.000");
//
//                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(p);
//                if (obj.transform.localScale != Vector3.one)
//                    error.scaleError = true;
//                if (obj.transform.localEulerAngles != Vector3.zero)
//                    error.angleError = true;
//
//                if (error.isError())
//                {
//                    error.fbxName = obj.name;
//                    errors.Add(error);
//                }
//            }
//
//            JsonWriter jw = new JsonWriter();
//            jw.PrettyPrint = true;
//            LitJson.JsonMapper.ToJson(errors, jw);
//            string path = EditorUtility.SaveFilePanel("导出FBX Error Checklist", Application.dataPath, "FBXErrorChecklist", "json");
//            if (!string.IsNullOrEmpty(path))
//            {
//                File.WriteAllText(path, jw.ToString());
//            }
//        }
//        finally
//        {
//            EditorUtility.ClearProgressBar();
//        }
//    }
//}
