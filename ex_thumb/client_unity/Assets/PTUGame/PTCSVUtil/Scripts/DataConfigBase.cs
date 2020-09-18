using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace PTGame.Utils.CVS
{
    public class DataConfigBase<T> : IDataConfigBase
	{

		public const string CommaCharacter = "@@@\t";

		public Dictionary<object,IConfigItem> itemSets = new Dictionary<object, IConfigItem> ();

		public static IDataConfigBase LoadConfigFile(string csvFile = null)
		{
			IDataConfigBase config = (IDataConfigBase)Activator.CreateInstance (typeof(T));

			if(csvFile == null)
			{
				csvFile = config.GetCsvFile ();
			}

			TextAsset txt = Resources.Load (csvFile) as TextAsset;

			config.InitData (txt.text);

			return config;
		}

		public static IDataConfigBase LoadConfig(string content)
		{
			IDataConfigBase config = (IDataConfigBase)Activator.CreateInstance (typeof(T));

			config.InitData (content);

			return config;
		}

		public void Load(string content)
		{
			InitData (content);
		}
			
		public void InitData (string csvContent)
		{
          
			Type configItemType = GetItemType ();


			string dataTxt = csvContent;

			dataTxt = dataTxt.Replace ("\r", "");

			string[] hList = Regex.Split(dataTxt,"@@@@@@\n",RegexOptions.IgnoreCase);
			
			
			string title = hList [2];
			string[] titles = Regex.Split(title,CommaCharacter,RegexOptions.IgnoreCase);

			string[] types = Regex.Split(hList [0],CommaCharacter,RegexOptions.IgnoreCase);



			for (int col = 3; col < hList.Length; col++) {
				IConfigItem dataItem = null;
				object key = null;

				string[] vals =  Regex.Split(hList [col],CommaCharacter,RegexOptions.IgnoreCase); 

				if (vals.Length != titles.Length) {
					continue;
				}

				dataItem = (IConfigItem)Activator.CreateInstance (configItemType);

				for (int row = 0; row < titles.Length; row++) {

					string titleName = titles [row];

					if (string.IsNullOrEmpty (titleName)) {
						continue;
					}

					string typeStr = types [row];
					string valStr = vals [row];

					if (string.IsNullOrEmpty (typeStr)) {
						continue;
					}
					titleName = titleName.Trim ();

					string propertyName = titleName.Substring (0, 1).ToUpper () + titleName.Substring (1);

					PropertyInfo prop = configItemType.GetProperty (propertyName, BindingFlags.Public | BindingFlags.Instance);
					object val = null;

					if (prop.PropertyType.Name == STR_TYPE_VECTOR3)
					{
						val = ConvertToV3 (valStr);

					}else if(prop.PropertyType.Name == STR_TYPE_LISTSTRING)
					{
						val = ConvertToList(typeStr, valStr);
					}
					else {
						
						if(!string.IsNullOrEmpty(valStr))
						{
							val = Convert.ChangeType (valStr, prop.PropertyType);
						}

					}

					if (!string.IsNullOrEmpty (valStr)) {
						prop.SetValue (dataItem, val, null);
					}


					if (row == 0) 
					{
						key = val;
					}

				}

				if (dataItem != null && key!=null) {
//					Debug.LogError (key+">>>>Key >>> "+col);
					itemSets.Add (key, dataItem);
				}
			}

		}

		private const string STR_TYPE_VECTOR3 = "Vector3";
		private const string STR_TYPE_LISTSTRING = "List`1";

		private static Vector3 ConvertToV3(string data)
		{
			if(string.IsNullOrEmpty(data))
			{
				return Vector3.zero;
			}
			string t = data.Replace ("(","").Replace(")","");
			string[] results = t.Split (':');
			return new Vector3 (float.Parse(results[0]),float.Parse(results[1]),float.Parse(results[2]));
		}

		/// <summary>
		/// List支持int、string
		/// </summary>
		/// <param name="type"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		private static object ConvertToList(string type, string data)
		{
			if (string.IsNullOrEmpty(data))
			{
				return null;
			}
			string[] res = data.Split(';');
			if (type.Contains("string"))
				return new List<string>(res);
			if (type.Contains("int"))
				return res.Select(t => int.Parse(t)).ToList();
			return null;
		}


		public virtual string GetCsvFile ()
		{
			return "";
		}


		/// <summary>
		/// Gets the type of the bean.Need overwrite.
		/// </summary>
		/// <returns>The bean type.</returns>
		protected virtual Type GetItemType ()
		{
			return null;
		}


		protected IConfigItem _GetItemById (object id)
		{
			if (itemSets.ContainsKey (id)) {
				return itemSets [id];
			} else {
				return null;
			}
		}

	}
}
