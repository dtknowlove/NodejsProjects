//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//using System.IO;
//using System.Text;
//
//
//namespace com.putao.paibloks.editor
//{
//	public class PEPrefabGenerator :EditorWindow 
//	{
//		[MenuItem("BlockTool/Generate Prefabs/Custom", false, GlobalDefine.Menu_Prefab)]
//		public static void Init()
//		{
//			PEPrefabGenerator m_instance = (PEPrefabGenerator) EditorWindow.GetWindow(typeof(PEPrefabGenerator), true);
//			m_instance.position = new Rect(Screen.width / 2, Screen.height / 2, 500, 500);
//			m_instance.Show();
//		}
//
//		private Vector2 scrollPos;
//		private List<BlockDataGroup> groups;
//		private string searchText;
//
//		private void OnEnable()
//		{
//			groups = BlockDataGroupFactory.Instance.GetAllGroups();
//			searchText = "";
//		}
//
//		void OnGUI()
//		{
//			GUILayout.Space(10);
//			
//			GUILayout.BeginHorizontal();
//			searchText = GUILayout.TextField(searchText, GUILayout.Width(400));
//			GUILayout.Label("Search", GUILayout.Width(50));
//			GUILayout.EndHorizontal();
//			
//			GUILayout.Space(10);
//			
//			scrollPos = GUILayout.BeginScrollView(scrollPos);
//			foreach (var group in groups)
//			{
//				if (!group.modelName.ToLower().Contains(searchText.ToLower()))
//					continue;
//				
//				GUILayout.BeginHorizontal("box");
//				GUILayout.Label(group.modelName, GUILayout.Width(250));
//				
//				string path = Path.Combine(BlockPath.Fbx(group.category, PolygonType.LOW), group.modelName + ".FBX");
//				path = path.Substring("Assets/".Length);
//				path = Path.Combine(Application.dataPath, path);
//				GUILayout.Label(File.GetLastWriteTime(path).ToString("yy-MM-dd HH:mm"), GUILayout.Width(120));
//				
//				if (GUILayout.Button("Create L Prefabs", GUILayout.Width(150)))
//				{
//					List<BlockDataGroup> list = new List<BlockDataGroup>();
//					list.Add(group);
//					PEPrefabGeneratorUtil.CreatePrefabs(list, PolygonType.LOW);
//				}
//				if (GUILayout.Button("Create H Prefabs", GUILayout.Width(150)))
//				{
//					List<BlockDataGroup> list = new List<BlockDataGroup>();
//					list.Add(group);
//					PEPrefabGeneratorUtil.CreatePrefabs(list, PolygonType.HIGH);
//				}
//				GUILayout.EndHorizontal();
//			}
//			GUILayout.EndScrollView();
//		}
//
//	
//
//		private static void DisplayErrorDialog(string title, List<string> errors)
//		{
//			StringBuilder sb = new StringBuilder(title);
//			foreach (string error in errors)
//			{
//				sb.AppendFormat("\n{0}.fbx", error);
//			}
//			EditorUtility.DisplayDialog("Error", sb.ToString(), "确定");
//		}
//
//		[MenuItem("BlockTool/Generate Prefabs/Low_1", false, GlobalDefine.Menu_Prefab)]
//		public static void CreatePrefabsLow()
//		{
//			PEPrefabGeneratorUtil.CreatePrefabs(BlockDataGroupFactory.Instance.GetGroupsByCategory(Category.large), PolygonType.LOW);
//		}
//
//		[MenuItem("BlockTool/Generate Prefabs/Low_fig", false, GlobalDefine.Menu_Prefab)]
//		public static void CreatePrefabsLowFig()
//		{
//			PEPrefabGeneratorUtil.CreatePrefabs(BlockDataGroupFactory.Instance.GetGroupsByCategory(Category.fig), PolygonType.LOW);
//		}
//
//		[MenuItem("BlockTool/Generate Prefabs/Low_pbs", false, GlobalDefine.Menu_Prefab)]
//		public static void CreatePrefabsLowPbs()
//		{
//			PEPrefabGeneratorUtil.CreatePrefabs(BlockDataGroupFactory.Instance.GetGroupsByCategory(Category.small), PolygonType.LOW);
//		}
//
//		[MenuItem("BlockTool/Generate Prefabs/Low_tech", false, GlobalDefine.Menu_Prefab)]
//		public static void CreatePrefabsLowTech()
//		{
//			PEPrefabGeneratorUtil.CreatePrefabs(BlockDataGroupFactory.Instance.GetGroupsByCategory(Category.tech), PolygonType.LOW);
//		}
//
//		[MenuItem("BlockTool/Generate Prefabs/High_1", false, GlobalDefine.Menu_Prefab)]
//		public static void CreatePrefabsHigh()
//		{
//			PEPrefabGeneratorUtil.CreatePrefabs(BlockDataGroupFactory.Instance.GetGroupsByCategory(Category.large, PolygonType.HIGH), PolygonType.HIGH);
//		}
//
//		[MenuItem("BlockTool/Generate Prefabs/High_fig", false, GlobalDefine.Menu_Prefab)]
//		public static void CreatePrefabsHighFig()
//		{
//			PEPrefabGeneratorUtil.CreatePrefabs(BlockDataGroupFactory.Instance.GetGroupsByCategory(Category.fig, PolygonType.HIGH), PolygonType.HIGH);
//		}
//
//		[MenuItem("BlockTool/Generate Prefabs/High_pbs", false, GlobalDefine.Menu_Prefab)]
//		public static void CreatePrefabsHighPbs()
//		{
//			PEPrefabGeneratorUtil.CreatePrefabs(BlockDataGroupFactory.Instance.GetGroupsByCategory(Category.small, PolygonType.HIGH), PolygonType.HIGH);
//		}
//
//		[MenuItem("BlockTool/Generate Prefabs/High_tech", false, GlobalDefine.Menu_Prefab)]
//		public static void CreatePrefabsHighTech()
//		{
//			PEPrefabGeneratorUtil.CreatePrefabs(BlockDataGroupFactory.Instance.GetGroupsByCategory(Category.tech, PolygonType.HIGH), PolygonType.HIGH);
//		}
//
//	}
//}