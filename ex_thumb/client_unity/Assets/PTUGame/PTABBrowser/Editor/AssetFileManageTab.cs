using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AssetBundleBrowser
{
	[System.Serializable]
	internal class AssetFileManageTab
	{
		private const string PATH_CONFIGFILE = "Assets/PTUGame/customconfig/abfiles.json";


		internal void OnEnable(EditorWindow parent)
		{
			if (!File.Exists(PATH_CONFIGFILE))
			{
				mItems = new List<ResFileItem>();
				return;
			}
			string content = File.ReadAllText(PATH_CONFIGFILE);
		
			ResFiles resFiles = JsonUtility.FromJson<ResFiles>(content);

			mItems = resFiles.items;
		}

		internal void OnDisable()
		{
			ResFiles  resFiles = new ResFiles();
			resFiles.items = mItems;
			string content = JsonUtility.ToJson(resFiles,true);
		
			File.WriteAllText(PATH_CONFIGFILE,content);
		
			AssetDatabase.Refresh();
		}

		private List<ResFileItem> mItems;

		public void OnGUI()
		{

			ResFileItem removedItem = null;
		
			GUILayout.BeginVertical();		
			foreach (var item in mItems)
			{
				GUILayout.BeginHorizontal();

				GUILayout.TextField(item.name,GUILayout.Width(150));
			
				GUILayout.TextField(item.assetPath,GUILayout.Width(250));

				if (GUILayout.Button("Remove"))
				{
					removedItem = item;
				}

				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();

			if (removedItem!=null)
			{
				mItems.Remove(removedItem);
			}
		
			DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
			if (Event.current.type == EventType.DragExited)
			{
				foreach (var t in DragAndDrop.objectReferences)
				{
					AddItem( AssetDatabase.GetAssetPath(t));
				}
			}
		}

		private void AddItem(string path)
		{
			ResFileItem resItem = new ResFileItem();

			resItem.name = Path.GetFileName(path);
			resItem.assetPath = path;
			mItems.Add(resItem);
		}

	}
	[System.Serializable]
	public class ResFileItem
	{
		public string name = "";
		public string assetPath = "";
		public string finalPath = "";
	}

	[System.Serializable]
	public class ResFiles
	{
		public List<ResFileItem> items;
	}
}