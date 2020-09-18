using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LitJson;
using PTGame.Core;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace com.putao.paibloks.editor
{
    public class PEThumbGenerator : EditorWindow
    {
        private string[] categoryNames = { "Category_large", "Category_largesuper", "Category_fig", "Category_pbs", "Category_tech" };
        private bool[] categoryToggles = {true, false, false, false, false};
        
        private int screenScaleFactor = 1;
        
        private string searchText;

        private string[] lightNames;
        private int lightIndex = 0;

        private ThumbInfo mThumInfo;

        private string mSavePath = "";
        
        private List<BlockDataGroup> blockGroups;
        private BlockDataGroup previewBlockGroup;
        
        private void OnEnable()
        {
            mThumInfo = new ThumbInfo();
            
            blockGroups = new List<BlockDataGroup>();
            for (int i = 0; i < categoryNames.Length; i++)
            {
                blockGroups.AddRange(BlockDataGroupFactory.Instance.GetGroupsByCategory((Category) i));
            }

            searchText = "";

            var colors = PBDataBaseManager.Instance.GetBlockColorDatas();
            colors.Sort((p1, p2) => String.Compare(p1.code, p2.code, StringComparison.Ordinal));
            var t = colors.Select(color => color.color).ToList();
            t.Insert(0, "所有");
            mColorNames = t.ToArray();
            
            mColorToggles = new bool[colors.Count + 1];
            for (int i = 0; i < mColorToggles.Length; i++)
                mColorToggles[i] = i == 0;
            
            UpdateLightNames();
        }

        private void OnFocus()
        {
            UpdateLightNames();
        }

        private void Update()
        {
            if (previewBlockGroup != null)
            {
//                PreviewThumb(previewBlockGroup);
            }
        }
       
        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            
            DrawSettings();
            
            GUILayout.BeginVertical();
            
            if (GUILayout.Button("Create Thumbs All", GUILayout.Width(200)))
            {
                CreateThumbForGroups(blockGroups);
            }
            
            GUILayout.BeginHorizontal();
            searchText = GUILayout.TextField(searchText, GUILayout.Width(400));
            GUILayout.Label("Search", GUILayout.Width(50));
            GUILayout.EndHorizontal();
            
            DrawItems();
            GUILayout.EndVertical();
            
            GUILayout.EndHorizontal();
        }

        private void DrawSettings()
        {
            GUILayout.BeginVertical(GUILayout.Width(250));
            
            GUILayout.Label("选择Category");
            for (int i = 0; i < categoryNames.Length; i++)
            {
                categoryToggles[i] = EditorGUILayout.ToggleLeft(categoryNames[i], categoryToggles[i], GUILayout.Width(150));
            }

            GUILayout.Space(20);
            
            GUILayout.Label("设置分辨率");
            screenScaleFactor = EditorGUILayout.IntSlider(screenScaleFactor, 1, 4, GUILayout.Width(150));
            GUILayout.Label(string.Format("缩率图分辨率{0} * {0}！",screenScaleFactor*240),GUILayout.Width(200));
            
            GUILayout.Space(20);
            
            DrawColorSelect();
            
            GUILayout.Space(20);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("保存路径", GUILayout.Width(60));
            if (GUILayout.Button("...", GUILayout.Width(50)))
            {
                string saveFolder = EditorUtility.SaveFolderPanel("", Application.dataPath.Replace("Assets", ""), "block_thumbs");
                if (!string.IsNullOrEmpty(saveFolder))
                    mSavePath = saveFolder;
            }
            GUILayout.EndHorizontal();
            GUILayout.Box(mSavePath, GUILayout.Width(250));
            
            GUILayout.EndVertical();
        }

        private string[] mColorNames;
        private bool[] mColorToggles;
        private Vector2 scrollPos_colorSelect = Vector2.zero;
        private void DrawColorSelect()
        {
            GUILayout.Label("选择颜色",GUILayout.Width(60));
            
            GUILayout.BeginVertical("helpBox");
            
            scrollPos_colorSelect = GUILayout.BeginScrollView(scrollPos_colorSelect, GUILayout.Width(240), GUILayout.Height(200));            
            GUILayout.BeginVertical();
            for (int i = 0; i < mColorNames.Length; i++)
            {
                bool curToggle = EditorGUILayout.ToggleLeft(mColorNames[i], mColorToggles[i], GUILayout.Width(200));
                if (curToggle != mColorToggles[i])
                {
                    mColorToggles[i] = curToggle;
                    if (curToggle)
                    {
                        if (i == 0)
                        {
                            for (int j = 1; j < mColorNames.Length; j++)
                            {
                                mColorToggles[j] = false;
                            }
                        }
                        else
                        {
                            mColorToggles[0] = false;
                        }
                        break;
                    }
                }
            }
            GUILayout.EndVertical();
            
            GUILayout.EndScrollView();

            GUILayout.Space(5);
            if (GUILayout.Button("为选中颜色保存灯光", GUILayout.Width(150)))
            {
                string[] selectColors = mColorNames.Where((color, index) => index > 0 && mColorToggles[index]).ToArray();
                if (selectColors.Length == 0)
                {
                    EditorUtility.DisplayDialog("", "请选择颜色！", "OK");
                }
                else if (selectColors.Length > 1)
                {
                    string msg = "确定为以下颜色保存这一套灯光吗？";
                    selectColors.ForEach(color => msg += "\n" + color);
                    if (EditorUtility.DisplayDialog("", msg, "OK", "Cancel"))
                    {
                        selectColors.ForEach(PEThumbLighting.SaveForColor);
                        UpdateLightNames();
                    }
                }
            }

            GUILayout.EndVertical();
        }

        private int IndexOfColor(string colorName)
        {
            for (int i = 0; i < mColorNames.Length; i++)
            {
                if (mColorNames[i].Equals(colorName))
                    return i;
            }
            return -1;
        }

        private Vector2 scrollPos;
        private void DrawItems()
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            foreach (var group in blockGroups)
            {
                if (!categoryToggles[(int)group.category])
                    continue;
                if (!group.modelName.ToLower().Contains(searchText.ToLower()))
                    continue;
                if (!mColorToggles[0] &&
                    !group.GetColors().Exists(color => IndexOfColor(color) >= 0 && mColorToggles[IndexOfColor(color)]))
                    continue;

                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                GUILayout.Label(group.modelName, GUILayout.Width(250));
                
                DrawAngles(group.modelName);
                
                if (GUILayout.Button("Create Thumbs", GUILayout.Width(100)))
                {
                    CreateThumbForGroups(new List<BlockDataGroup>() {group});
                }
                if (GUILayout.Button("Preview", GUILayout.Width(60)))
                {
                    previewBlockGroup = group;
                    PreviewThumb(group);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }
       
        private const int DEFAULT_ANGLE = -35;
        
        private void DrawAngles(string modelName)
        {
            int[] angle = mThumInfo.GetAngleOfModel(modelName);
            bool oldToggle = mThumInfo.IsCustomAngle(angle);
            bool newToggle = EditorGUILayout.ToggleLeft("修改角度", oldToggle, GUILayout.Width(80));
            if (oldToggle != newToggle)
            {
                angle = new[] {0, newToggle ? 0 : DEFAULT_ANGLE, 0};
                mThumInfo.SetAngleOfModel(modelName, angle);
            }

            if (newToggle)
            {
                GUILayout.Label("x", GUILayout.Width(10));
                int newAngleX = EditorGUILayout.IntField(angle[0], GUILayout.Width(60));
                if (angle[0] != newAngleX)
                {
                    mThumInfo.SetAngleOfModel(modelName, new[] {newAngleX, angle[1], angle[2]});
                }
                
                GUILayout.Label("y", GUILayout.Width(10));
                int newAngleY = EditorGUILayout.IntField(angle[1], GUILayout.Width(60));
                if (angle[1] != newAngleY)
                {
                    mThumInfo.SetAngleOfModel(modelName, new[] {angle[0], newAngleY, angle[2]});
                }

                GUILayout.Label("z", GUILayout.Width(10));
                int newAngleZ = EditorGUILayout.IntField(angle[2], GUILayout.Width(60));
                if (angle[2] != newAngleZ)
                {
                    mThumInfo.SetAngleOfModel(modelName, new[] {angle[0], angle[1], newAngleZ});
                }
            }
            else
            {
                GUILayout.Space(65);
            }
        }

        private void PrepareThumbExportElement(BlockDataGroup group, List<ThumbExportElement> elements)
        {
            if (!categoryToggles[(int) group.category])
                return;

            string prefabDir = BlockPath.Prefab(group.category, PolygonType.HIGH);
            int[] angle = mThumInfo.GetAngleOfModel(group.modelName);

            List<string> prefabs = new List<string>();
            if (!mColorToggles[0])
            {
                foreach (string color in group.GetColors())
                {
                    int indexOfColor = IndexOfColor(color);
                    if (indexOfColor >= 0 && mColorToggles[indexOfColor])
                        prefabs.AddRange(group.GetPrefabsByColor(color));
                }
            }
            else
            {
                prefabs.AddRange(group.GetPrefabs());
            }

            foreach (string prefab in prefabs)
            {
                BlockDataItem dataItem = group.GetItems().FirstOrDefault(s => s.prefabName == prefab);
                elements.Add(new ThumbExportElement()
                {
                    eulerAngle = new Vector3(angle[0], angle[1], angle[2]),
                    prefabPath = Path.Combine(prefabDir, prefab + ".prefab"),
                    modelName = group.modelName,
                    material = dataItem.material,
                    materialH = dataItem.materialHigh,
                    category = group.category,
                    light = lightNames.Contains(dataItem.colorName) ? dataItem.colorName : "",
                });
            }
        }

        private void CreateThumbForGroups(List<BlockDataGroup> groups)
        {
            previewBlockGroup = null;
            
            if (groups.Count == 0)
            {
                EditorUtility.DisplayDialog("", "Nothing to export!!!", "OK");
                return;
            }

            if (string.IsNullOrEmpty(mSavePath))
            {
                EditorUtility.DisplayDialog("", "请指定保存路径！！！", "OK");
                return;
            }
            
            if (!string.Equals(SceneManager.GetActiveScene().name, "GenerateThumb"))
            {
                bool ok = EditorUtility.DisplayDialog("", "将打开场景：GenerateThumb，当前场景需要保存吗？", "保存", "舍弃");
                if (ok)
                {
                    EditorSceneManager.SaveOpenScenes();
                }
                EditorSceneManager.OpenScene("Assets/BlockEditor/Scenes/GenerateThumb.unity");
            }

            List<ThumbExportElement> elements = new List<ThumbExportElement>();
            groups.ForEach(group => PrepareThumbExportElement(group, elements));

            ThumbExportInfo info = new ThumbExportInfo();
            info.thumbs = elements.ToArray();
            info.scaleFactor = screenScaleFactor;
            info.saveFolder = mSavePath;

            PEThumbExporter exporter = GameObject.FindObjectOfType<PEThumbExporter>();
            exporter.SetExportInfo(info);

            FindObjectOfType<PEThumbExporter>().StartRender();
        }

        private void PreviewThumb(BlockDataGroup group)
        {
//            string[] selectColors = mColorNames.Where((color, index) => mColorToggles[index]).ToArray();
//            if (selectColors.Length != 1 || mColorToggles[0])
//            {
//                EditorUtility.DisplayDialog("", "只能预览一个颜色！", "OK");
//                previewBlockGroup = null;
//                return;
//            }
            
            if (!string.Equals(SceneManager.GetActiveScene().name, "GenerateThumb"))
            {
                bool ok = EditorUtility.DisplayDialog("", "将打开场景：GenerateThumb，当前场景需要保存吗？", "保存", "舍弃");
                if (ok)
                {
                    EditorSceneManager.SaveOpenScenes();
                }
                EditorSceneManager.OpenScene("Assets/BlockEditor/Scenes/GenerateThumb.unity");
            }

            List<ThumbExportElement> elements = new List<ThumbExportElement>();
            PrepareThumbExportElement(group, elements);
            
            ThumbExportInfo info = new ThumbExportInfo();
            info.thumbs = elements.ToArray();
            info.scaleFactor = screenScaleFactor;
            info.saveFolder = mSavePath;

            PEThumbExporter exporter = GameObject.FindObjectOfType<PEThumbExporter>();
            exporter.SetExportInfo(info);

            FindObjectOfType<PEThumbExporter>().StartPreview();
        }

        private void UpdateLightNames()
        {
            string[] prefabFiles = Directory.GetFiles(Path.Combine(Application.dataPath, "BlockEditor/Resources/ThumbLight/"), "*.prefab");
            lightNames = prefabFiles.Select(file =>
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                return fileName.Equals("pblight") ? "通用" : fileName.Substring("pblight_".Length);
            }).ToArray();
        }
    


        #region 缩略图配置

        class ThumbInfoItem
        {
            public string modelName;
            public int angleY;    //angleY, 兼容老数据
            public int angleX;
            public int angleZ;
        }

        private class ThumbInfo
        {
            private List<ThumbInfoItem> items;

            private string filePath;
            public ThumbInfo()
            {
                filePath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "ThumbInfoConfig");
                filePath.CreateDirIfNotExists();
                filePath = Path.Combine(filePath, "export_thumb_info.json");

                LoadThumbInfo();
            }

            private void LoadThumbInfo()
            {
                if (File.Exists(filePath))
                {
                    string text = File.ReadAllText(filePath);
                    items = LitJson.JsonMapper.ToObject<List<ThumbInfoItem>>(text);
                }
                else
                {
                    items = new List<ThumbInfoItem>();
                }
            }

            public void AddItem(string modelName,int x,int y,int z)
            {
                items.RemoveAll(s => s.modelName == modelName);
                items.Add(new ThumbInfoItem()
                {
                    modelName = modelName,
                    angleX =x,
                    angleY = y,
                    angleZ = z
                });

                SaveFile();
            }
            
            public void SetAngleOfModel(string modelName, int[] angle)
            {
                if (!IsCustomAngle(angle))
                {
                    RemoveItemWithName(modelName);
                }
                else
                {
                    AddItem(modelName,angle[0],angle[1],angle[2]);
                }
            }

            public void RemoveItemWithName(string modelName)
            {
                items.RemoveAll(s => s.modelName == modelName);

                SaveFile();
            }

            private void SaveFile()
            {
                JsonWriter jw = new JsonWriter {PrettyPrint = true};
                LitJson.JsonMapper.ToJson(items, jw);
                File.WriteAllText(filePath, jw.ToString());
            }

            public int[] GetAngleOfModel(string modelName)
            {
                var t = items.FirstOrDefault(s => s.modelName == modelName);
                if (t == null)
                {
                    return  new[] {0, DEFAULT_ANGLE, 0};
                }
                return new[]{t.angleX,t.angleY,t.angleZ};
            }
            public bool IsCustomAngle(int[] angle)
            {
                return !(angle[0] == 0 && angle[1] == DEFAULT_ANGLE && angle[2] == 0);
            }
        }
        
        #endregion
    }
}
