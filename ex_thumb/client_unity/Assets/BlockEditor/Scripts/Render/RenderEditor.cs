using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class RenderEditor
{
    [MenuItem("BlockTool/Render2")]
    public static void ExecCmd()
    {
        string openpara = string.Empty;
        string[] ss = Environment.GetCommandLineArgs();
        for (int i = 0; i < ss.Length; i++)
        {
            if (ss[i] == "-openpara")
            {
                openpara = ss[i + 1];
            }
        }
        Debug.Log("Call RenderEditor.ExecCmd by " + openpara);
        PlayerPrefs.SetString(RenderThumbMgr.Key_DataFile, openpara);
        RenderThumbMgr.ReportSegTime(2, "ToPlaying");
		
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene("Assets/BlockEditor/Scenes/GenerateThumb.unity");
        EditorApplication.isPlaying = true;
    }
}
