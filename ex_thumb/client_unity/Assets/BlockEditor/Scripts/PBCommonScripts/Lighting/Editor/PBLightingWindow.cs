using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PBLightingWindow : EditorWindow
{
#if BLOCK_EDITOR
    [MenuItem("BlockTool/搭建灯光编辑", false, BlockEditorGlobal.Light_Edit_Window)]
#endif
    static void Init()
    {
        PBLightingWindow window = EditorWindow.GetWindow<PBLightingWindow>(false, "Light Edit Window");
        window.autoRepaintOnSceneChange = true;
        window.Show();
    }

    private string[] lightNames_Formal;
    private string[] lightNames_Beta;

    private string lightApplied;

    private void OnEnable()
    {
        UpdateLightNames();
        lightApplied = PBLightingLiteracy.ReadLocal();
    }

    private void OnFocus()
    {
        UpdateLightNames();
        lightApplied = PBLightingLiteracy.ReadLocal();
    }

    private int tabIndex = 0;
    private string[] tabNames = {"正式版", "测试版"};

    private void OnGUI()
    {
        tabIndex = GUILayout.Toolbar(tabIndex, tabNames);

        if (tabIndex == 0)
            DrawLights_Formal();
        else if (tabIndex == 1)
            DrawLights_Beta();

        if (GUILayout.Button("删除场景中灯光"))
            DeletePBLight();
    }

    private const float CELL_SPACE = 50;
    private const float CELL_HEIGHT = 40;
    
    private Color oriBgColor;
    
    private Vector2 scrollPos = Vector2.zero;

    private void DrawLights_Formal()
    {
        GUILayout.Space(10);
        scrollPos = GUILayout.BeginScrollView(scrollPos);

        for (int i = 0; i < lightNames_Formal.Length; i++)
        {
            string lightName = lightNames_Formal[i];

            oriBgColor = GUI.backgroundColor;
            GUI.backgroundColor = lightName.Equals(lightApplied) ? Color.green : oriBgColor;
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUI.backgroundColor = oriBgColor;
            
            if (GUILayout.Button("预览", GUILayout.Width(60)))
            {
                PreviewLight(PBLighting.FormalPath, lightName);
            }
            if (GUILayout.Button("应用", GUILayout.Width(60)))
            {
                PBLightingLiteracy.SaveLocal(lightName);
                lightApplied = lightName;
            }
            GUILayout.Label(lightName, GUILayout.Width(200));
            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();

    }
    
    private Vector2 scrollPos_Beta = Vector2.zero;
    private string lightNameSaved = "";

    private void DrawLights_Beta()
    {
        GUILayout.Space(5);
        
        GUILayout.BeginHorizontal();
        GUILayout.Label("保存灯光", GUILayout.Width(50));
        lightNameSaved = GUILayout.TextField(lightNameSaved, 20, GUILayout.Width(150));
        if (GUILayout.Button("+", GUILayout.Width(80)))
        {
            SaveNewLight();
        }
        GUILayout.EndHorizontal();
        
        GUILayout.Space(5);
        scrollPos_Beta = GUILayout.BeginScrollView(scrollPos_Beta);
        
        for (int i = 0; i < lightNames_Beta.Length; i++)
        {
            string lightName = lightNames_Beta[i];
            
            oriBgColor = GUI.backgroundColor;
            GUI.backgroundColor = lightName.Equals(lightApplied) ? Color.green : oriBgColor;
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUI.backgroundColor = oriBgColor;
            
            if (GUILayout.Button("-", GUILayout.Width(30)))
            {
                RemoveLight(lightName);
            }
            if (GUILayout.Button("预览", GUILayout.Width(60)))
            {
                PreviewLight(PBLighting.BetaPath, lightName);
            }
            if (GUILayout.Button("应用", GUILayout.Width(60)))
            {
                PBLightingLiteracy.SaveLocal(lightName);
                lightApplied = lightName;
            }
            GUILayout.Label(lightName, GUILayout.Width(200));
            GUILayout.EndHorizontal();
        }
        
        GUILayout.EndScrollView();
    }

    private void UpdateLightNames()
    {
        lightNames_Formal = Directory.GetFiles(PBLighting.FormalPath, "*.json")
            .Select(file => Path.GetFileNameWithoutExtension(file)).ToArray();

        if (Directory.Exists(PBLighting.BetaPath))
        {
            lightNames_Beta = Directory.GetFiles(PBLighting.BetaPath, "*.json")
                .Select(file => Path.GetFileNameWithoutExtension(file)).ToArray();
        }
        else
        {
            lightNames_Beta = new string[0];
        }
    }

    private void SaveNewLight()
    {
        if (string.IsNullOrEmpty(lightNameSaved))
        {
            EditorUtility.DisplayDialog("保存灯光", "请输入新增灯光的名字！", "OK");
            return;
        }
        if (lightNames_Formal.Contains(lightNameSaved))
        {
            EditorUtility.DisplayDialog("保存灯光", string.Format("{0} 已存在于正式版中，不可覆盖！", lightNameSaved), "OK");
            return;
        }
        if (lightNames_Beta.Contains(lightNameSaved))
        {
            if (!EditorUtility.DisplayDialog("保存灯光", "是否替换已有灯光：\n" + lightNameSaved, "OK", "Cancel"))
                return;
        }


        if (!Directory.Exists(PBLighting.BetaPath))
            Directory.CreateDirectory(PBLighting.BetaPath);
        
        string data = PBLighting.Serialize();
        File.WriteAllText(Path.Combine(PBLighting.BetaPath, lightNameSaved + ".json"), data);

        AssetDatabase.Refresh();
        UpdateLightNames();
        lightNameSaved = "";
    }

    private void RemoveLight(string lightName)
    {
        File.Delete(Path.Combine(PBLighting.BetaPath, lightName + ".json"));
        AssetDatabase.Refresh();
        UpdateLightNames();
    }

    private void PreviewLight(string path, string lightName)
    {
        string data = File.ReadAllText(Path.Combine(path, lightName + ".json"));
        PBLighting.Load(data);
    }

    private void DeletePBLight()
    {
        GameObject lightObj = GameObject.Find(PBLighting.NAME);
        if (lightObj != null)
            GameObject.DestroyImmediate(lightObj);
    }

    private static Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }

}
