using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using PTGame.Core;
using UniRx;
using UnityEditor;
using UnityEngine;

namespace Block.Editor
{
	public static class BlockServerUtil
	{
		private static string UrlOnline = "https://api-blocks.putao.com/blocks/title/list";
		private static string UrlTest = "http://api-blocks.test.ptevent.cn/blocks/title/list";

		public const string DIR_SkuData = "SkuData";

		private static string OnlineDataPath
		{
			get { return DIR_SkuData+"/skudataonline.json"; }
		}
		
		private static string TestDataPath
		{
			get { return DIR_SkuData+"/skudatatest.json"; }
		}

		private const string DIR_BuildAnim = "tmpbuildanim";
		private static string OnlineBuildPath
		{
			get { return DIR_BuildAnim+"/buildonline"; }
		}
		
		private static string TestBuildPath
		{
			get { return DIR_BuildAnim + "/buildtest"; }
		}

		public static void RequestData(bool isOnline, Action<bool> callBack)
		{
			ObservableWWW.Get(isOnline ? UrlOnline : UrlTest).Subscribe(result =>
			{
				if (!result.IsNullOrEmpty())
				{
					var data = ResponseData.Parse(result);
					if (data != null)
					{
						data.Sort();
						DIR_SkuData.CreateDirIfNotExists();
						var finalPath = GetSkuDataPath(isOnline);
						finalPath.DeleteFileIfExists();
						File.WriteAllText(finalPath, JsonUtility.ToJson(data, true));
						DownloadFiles(data.data.ToList(), isOnline);
						callBack.InvokeGracefully(true);
						return;
					}
				}
				callBack.InvokeGracefully(false);
			}, e =>
			{
				Debug.LogError(e);
				callBack.InvokeGracefully(false);
			});
		}

		public static List<SkuData> GetBlokDatas(bool isOnline)
		{
			var finalPath = GetSkuDataPath(isOnline);
			if (!File.Exists(finalPath))
			{
				Debug.LogError(finalPath + "不存在！");
				return null;
			}
			var data = ResponseData.Parse(File.ReadAllText(finalPath));
			if (data != null)
				return data.data.ToList();
			return null;
		}

		public static List<SkuCarData> SkuCarDatas(this List<SkuData> data)
		{
			var result = new List<SkuCarData>();
			data.ForEach(t =>
			{
				result.AddRange(t.Models);
			});
			return result;
		}
		
		public static void DownloadFiles(List<SkuData> datas,bool isOnline)
		{
			var dir = GetBuildAnimPath(isOnline);
			dir.CreateDirIfNotExists();

			//检查需要下载项
			var cars = datas.SkuCarDatas();
			var hashFile = GetHashFile(dir);
			var needUpdateDatas = new List<SkuCarData>();
			var find = false;
			foreach (var t in cars)
			{
				if (t.config_file.IsNullOrEmpty() || !t.config_file.StartsWith("http"))
				{
					Debug.LogErrorFormat("下载链接异常,请检查{0}后台是否上传文件:{1}", isOnline ? "线上环境" : "测试环境", t);
					find = true;
					continue;
				}
				var filePath = string.Format("./{0}/{1}.txt", dir, t.model_sku_id);
				var hash = t.config_file.Substring(t.config_file.LastIndexOf('/') + 1).Split('.')[0];
				bool updated = File.Exists(filePath) && hashFile.data.Exists(e => e.file == t.model_sku_id && e.hash == hash);
				if (!updated)
					needUpdateDatas.Add(t);
			}
			if (needUpdateDatas.Count <= 0)
			{
				Debug.Log(find ? "===>>数据有异常，请查看输出日志!" : "===>>本地为最新，无需下载!");
				return;
			}
			
			WebClient client = new WebClient();
			var fileName = string.Empty;
			var curCount = 0;
			var maxCount = needUpdateDatas.Count;
			foreach (var t in needUpdateDatas)
			{
				fileName = dir + "/" + t.model_sku_id + ".txt";
				curCount++;
				if (EditorUtility.DisplayCancelableProgressBar("下载搭建文件", string.Format("下载:{0} {1}/{2}", fileName, curCount, maxCount), curCount * 1.0f / maxCount))
					break;
				client.DownloadFile(t.config_file, fileName);
				var configHash = t.config_file.Substring(t.config_file.LastIndexOf('/') + 1).Split('.')[0];
				var record = hashFile.data.Find(e => e.file == t.model_sku_id);
				if (record != null)
					record.hash = configHash;
				else
					hashFile.data.Add(new ConfigHash {file = t.model_sku_id, hash = configHash});
				SaveHashFile(dir, hashFile);
			}
			client.Dispose();
			EditorUtility.ClearProgressBar();
		}

		private static string GetSkuDataPath(bool isOnline)
		{
			return isOnline ? OnlineDataPath : TestDataPath;
		}
		
		public static string GetBuildAnimPath(bool isOnline)
		{
			return isOnline ? OnlineBuildPath : TestBuildPath;
		}
		
		private const string HASH_FILE = "_config_hash_table.json";
		private static ConfigHashFile GetHashFile(string dir)
		{
			string hashPath = Path.Combine(dir, HASH_FILE);
			if (File.Exists(hashPath))
				return JsonUtility.FromJson<ConfigHashFile>(File.ReadAllText(hashPath));
			return new ConfigHashFile() {data = new List<ConfigHash>()};
		}

		private static void SaveHashFile(string dir, ConfigHashFile file)
		{
			string hashPath = Path.Combine(dir, HASH_FILE);
			File.WriteAllText(hashPath, JsonUtility.ToJson(file, true));
		}

		[Serializable]
		class ConfigHash
		{
			public string file;
			public string hash;
		}

		/// <summary>
		/// 用来记录configfile是否为最新的，是否需要重新下载
		/// </summary>
		[Serializable]
		class ConfigHashFile
		{
			public List<ConfigHash> data;
		}
	}



	#region 网络数据

	[Serializable]
	public class ResponseData
	{
		public string code = "";
		public SkuData[] data;

		public bool IsDataNull
		{
			get { return data != null && data.Length <= 0; }
		}

		public void Sort()
		{
			if (IsDataNull)
				return;
			Array.Sort(data, (a, b) =>
			{
				if (a.block_sku_id < b.block_sku_id) return -1;
				if (a.block_sku_id > b.block_sku_id) return 1;
				return 0;
			});
		}

		public static ResponseData Parse(string content)
		{
			ResponseData resultData = null;
			try
			{
				resultData = JsonUtility.FromJson<ResponseData>(content);
			}
			catch (Exception e)
			{
				PTDebug.LogError(string.Format("Type:{0} 解析出错,解析数据:{1}", "ResponseData", content));
				return null;
			}
			return resultData;
		}
	}

	[Serializable]
	public class SkuData
	{
		public int block_sku_id;
		public SkuCarData[] models;
		public string title;

		private SkuCarData[] mPrivateModels = null;

		public SkuCarData[] Models
		{
			get { return mPrivateModels ?? (mPrivateModels = models.Where(t => t.FilterSame(block_sku_id.ToString())).ToArray()); }
		}

		public override string ToString()
		{
			return string.Format("SkuID:{0},Name:{1},Count:{2}", block_sku_id.ToString().ColorFormat(Color.green), title.ColorFormat(Color.green), Models.Length.ToString().ColorFormat(Color.green));
		}
	}

	[Serializable]
	public class SkuCarData
	{
		public string model_sku_id;
		public string title;
		public string config_file;

		public override string ToString()
		{
			return string.Format("Config:{0},Name:{1},Url:{2}", model_sku_id.ColorFormat(Color.green), title.ColorFormat(Color.green), config_file);
		}

		public bool FilterSame(string skuid)
		{
			return true;
			var tmp = model_sku_id.Split('_');
			if (char.IsDigit(tmp[1][0]))
			{
				return tmp[1].Equals(skuid) || tmp[1].Equals("01012");
			}
			if (char.IsLetter(tmp[1][0]))
			{
				return true;
			}
			return false;
		}
	}

	public static class DataExtension
	{
		public static string ColorFormat(this string text, Color color)
		{
			return String.Format("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGBA(color), text);
		}
	}

	#endregion
}
