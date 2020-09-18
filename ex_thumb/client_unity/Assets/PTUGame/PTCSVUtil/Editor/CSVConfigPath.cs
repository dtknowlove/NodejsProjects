using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using PTGame.Editor.PluginManager;

namespace PTGame.Utils.CVS
{
	public class CSVConfigPath :ScriptableObject {

		public Object csvDir;
		public Object classDir;


		private const string FILEPATH_CONFIG = "Assets/PTUGame/customconfig/ptcsvconfig.asset";
		public static CSVConfigPath  GetConfigPath()
		{
			if (!File.Exists (FILEPATH_CONFIG))
			{
				CSVConfigPath nameInfoObj = ScriptableObject.CreateInstance<CSVConfigPath>();

				UnityEditor.AssetDatabase.CreateAsset(nameInfoObj, FILEPATH_CONFIG);
			
				AssetDatabase.Refresh ();

				AssetDatabase.SaveAssets ();

			}
			
			CSVConfigPath configPath = AssetDatabase.LoadAssetAtPath<CSVConfigPath> (FILEPATH_CONFIG) as CSVConfigPath;

			return configPath;
		
		}

	}
}
