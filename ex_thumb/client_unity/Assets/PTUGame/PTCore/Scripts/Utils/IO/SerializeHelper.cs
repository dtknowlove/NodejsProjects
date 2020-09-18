/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
	using System;
	using System.IO;
	using System.Xml.Serialization;
	using System.Text;
	using UnityEngine;

	public static class SerializeHelper
	{
		public static bool SerializeBinary(string path, object obj)
		{
			if (string.IsNullOrEmpty(path))
			{
				Debug.LogWarning("SerializeBinary Without Valid Path.");
				return false;
			}

			if (obj == null)
			{
				Debug.LogWarning("SerializeBinary obj is Null.");
				return false;
			}

			using (var fs = new FileStream(path, FileMode.OpenOrCreate))
			{
				var bf =
					new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
				bf.Serialize(fs, obj);
				return true;
			}
		}

		public static object DeserializeBinary(Stream stream)
		{
			if (stream == null)
			{
				Debug.LogWarning("DeserializeBinary Failed!");
				return null;
			}

			using (stream)
			{
				var bf =
					new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
				var data = bf.Deserialize(stream);

				// TODO:这里没风险嘛?
				return data;
			}
		}

		public static object DeserializeBinary(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				Debug.LogWarning("DeserializeBinary Without Valid Path.");
				return null;
			}

			var fileInfo = new FileInfo(path);

			if (!fileInfo.Exists)
			{
				Debug.LogWarning("DeserializeBinary File Not Exit.");
				return null;
			}

			using (var fs = fileInfo.OpenRead())
			{
				var bf =
					new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
				var data = bf.Deserialize(fs);

				if (data != null)
				{
					return data;
				}
			}

			Debug.LogWarning("DeserializeBinary Failed:" + path);
			return null;
		}

		public static bool SerializeXML(string path, object obj)
		{
			if (string.IsNullOrEmpty(path))
			{
				Debug.LogWarning("SerializeBinary Without Valid Path.");
				return false;
			}

			if (obj == null)
			{
				Debug.LogWarning("SerializeBinary obj is Null.");
				return false;
			}

			using (var fs = new FileStream(path, FileMode.OpenOrCreate))
			{
				var xmlserializer = new XmlSerializer(obj.GetType());
				xmlserializer.Serialize(fs, obj);
				return true;
			}
		}

		/// <summary>  
		/// 字节转string  
		/// </summary>  
		/// <param name="b">字节数组</param>  
		/// <returns></returns>  
		public static string UTF8ByteArrayToString(byte[] b)
		{
			UTF8Encoding encoding = new UTF8Encoding();
			string s = encoding.GetString(b);
			return (s);
		}

		/// <summary>  
		/// 字符串转字节数组  
		/// </summary>  
		/// <param name="s">字符内容</param>  
		/// <returns></returns>  
		public static byte[] StringToUTF8ByteArray(string s)
		{
			var encoding = new UTF8Encoding();
			var b = encoding.GetBytes(s);
			return b;
		}

		/// <summary>  
		/// 反序列化  
		/// </summary>  
		/// <param name="xmlString">string内容</param>  
		/// <param name="t">类型</param>  
		/// <returns></returns>  
		public static T DeserializeContent2XmlObj<T>(this string xmlString) where T : class
		{
			var ms = new MemoryStream(StringToUTF8ByteArray(xmlString));
			var xs = new XmlSerializer(typeof(T));
			return xs.Deserialize(ms) as T;
		}

		public static T DeserializePath2XmlObj<T>(this string path) where T : class 
		{
			if (string.IsNullOrEmpty(path) || !File.Exists(path))
			{
				Debug.LogWarning("DeserializeBinary Without Valid Path.");
				return default(T);
			}

			var fileInfo = new FileInfo(path);

			using (var fs = fileInfo.OpenRead())
			{
				var xmlserializer = new XmlSerializer(typeof(T));
				var data = xmlserializer.Deserialize(fs);

				if (data != null)
				{
					return data as T;
				}
			}

			Debug.LogWarning("DeserializeBinary Failed:" + path);
			return default(T);
		}

		public static string ToJson<T>(this T obj, bool prettyPrint = false) where T : class
		{
			return JsonUtility.ToJson(obj, prettyPrint);
		}

		public static T FromJson<T>(this string json) where T : class
		{
			try
			{
				return JsonUtility.FromJson<T>(json);
			}
			catch (Exception e)
			{
				PTDebug.LogError("{0} 参数解析错误:{1} 数据:{2}", typeof(T), e.Message, json);
				return null;
			}
		}

		public static void SaveJson<T>(this T obj, string path) where T : class
		{
			File.WriteAllText(path, obj.ToJson<T>());
		}

		public static T LoadJson<T>(string path) where T : class
		{
			return File.ReadAllText(path).FromJson<T>();
		}

		public static byte[] ToProtoBuff<T>(this T obj) where T : class
		{
			using (var ms = new MemoryStream())
			{
				ProtoBuf.Serializer.Serialize<T>(ms, obj);
				return ms.ToArray();
			}
		}

		public static T FromProtoBuff<T>(this byte[] bytes) where T : class
		{
			if (bytes == null || bytes.Length == 0)
			{
				throw new System.ArgumentNullException("bytes");
			}
			var t = ProtoBuf.Serializer.Deserialize<T>(new MemoryStream(bytes));
			return t;
		}

		public static void SaveProtoBuff<T>(this T obj, string path) where T : class
		{
			File.WriteAllBytes(path, obj.ToProtoBuff<T>());
		}

		public static T LoadProtoBuff<T>(string path) where T : class
		{
			return File.ReadAllBytes(path).FromProtoBuff<T>();
		}
	}
}