	using System;
	using System.IO;
	using PTGame.Core;
	using UniRx;
	using UnityEngine;

/// <summary>
/// 上传工具
/// </summary>
public class UploadUtility
{
	public const string Upload_File = "https://putao-uploader.oss-cn-hangzhou.aliyuncs.com/";
	public const string Upload_FileInfo = "https://api-uploader.putao.com/";
	/// <summary>
	/// 获取文件前缀
	/// </summary>
	private static string ConstFileUrlPrefix
	{
		get { return Upload_File + "file/"; }
	}

	private static string ConstOriginUrlPrefix
	{
		get { return Upload_FileInfo; }
	}

	private static string UploadAppId = "1003";
	private static string UploadSecret = "Gd7evhFLFCB2CUwZ4e1isGjYw6wuME4V";

	public static IDisposable UploadFile(string filePath, Action<string,string> onCallback = null)
	{
		if (!File.Exists(filePath))
		{
			PTDebug.LogError("文件不存在！路径:{0}", filePath);
			onCallback.InvokeGracefully(filePath,null);
			return null;
		}
		return GetUploadToken(token =>
		{
			if (token.IsNullOrEmpty())
			{
				PTDebug.LogError("获取UploadToken失败！");
				onCallback.InvokeGracefully(filePath,null);
				return;
			}
			var data = GetUploadFliePostData(filePath, token);
			ObservableWWW.Post(data.Url, data.Form).Timeout(TimeSpan.FromSeconds(30)).Subscribe(res =>
			{
				var resData = RespUploadData.Parse(res);
				if (resData.code == 0)
				{
					onCallback.InvokeGracefully(filePath,RespUploadData.GetFileName(res));
				}
				else
				{
					PTDebug.LogError(resData.message);
					onCallback.InvokeGracefully(filePath,null);
				}
			}, e =>
			{
				PTDebug.LogError("Error:" + e.ToString());
				onCallback.InvokeGracefully(filePath,null);
			});
		});
	}

	public static IDisposable DownloadTexture(string filename, Action<Texture2D> onCallback)
	{
		var url = GetDownloadFileUrl(filename);
		var dispose = ObservableWWW.GetAndGetBytes(url).Timeout(TimeSpan.FromSeconds(60)).Subscribe(bytes =>
		{
			PTDebug.Log("图片下载成功,url:" + url);
			var tex = new Texture2D(10, 10);
			tex.LoadImage(bytes);
			onCallback.InvokeGracefully(tex);
		}, e =>
		{
			PTDebug.LogError("Error,url:{0} error:{1}", url, e.ToString());
			onCallback.InvokeGracefully(null);
		});
		return dispose;
	}

	public static string GetDownloadFileUrl(string fileName)
	{
		return ConstFileUrlPrefix + fileName;
	}

	public static IDisposable GetUploadToken(Action<string> onCallback)
	{
		var tokenUrl = GetUploadTokenUrl();
		PTDebug.Log(tokenUrl);
		return ObservableWWW.Get(tokenUrl).Timeout(TimeSpan.FromSeconds(10)).Subscribe(res =>
		{
			var resData = TokenData.Parse(res);
			if (resData.code == 0)
			{
				onCallback.InvokeGracefully(TokenData.GetUploadToken(res));
			}
			else
			{
				PTDebug.LogError(resData.message);
				onCallback.InvokeGracefully(null);
			}
		}, e =>
		{
			PTDebug.LogError("Error:" + e.ToString());
			onCallback.InvokeGracefully(null);
		});
	}

	/// <summary>
	/// 获取tokenURL
	/// </summary>
	public static string GetUploadTokenUrl()
	{
		var appid = UploadAppId;
		var secret = UploadSecret;
		var rnd = GenerateRnd(8);
		var timestamp = GetTimeStamp();
		var signature = Md5(string.Format("appId={0}&rnd={1}&timestamp={2}{3}", appid, rnd, timestamp, secret));
		return string.Format("{0}upload/token?appId={1}&rnd={2}&signature={3}&timestamp={4}", ConstOriginUrlPrefix, appid, rnd, signature, timestamp);
	}

	/// <summary>
	/// 上报编程postdata
	/// </summary>
	public static PostData GetUploadFliePostData(string filePath, string token)
	{
		var result = new PostData();
		result.SetUrl(ConstOriginUrlPrefix + "upload/single");
		var form = new WWWForm();
		form.AddField("appId", UploadAppId);
		form.AddBinaryData("file", File.ReadAllBytes(filePath));
		form.AddField("uploadToken", token);
		result.SetForm(form);
		return result;
	}

	/// <summary>
	/// 加密 规则:abc=test1&abb=test2&abc=test3secret
	/// </summary>
	private static string Md5(string data)
	{
		var bytes = System.Text.Encoding.UTF8.GetBytes(data);
		var hash = System.Security.Cryptography.MD5.Create().ComputeHash(bytes);

		var builder = new System.Text.StringBuilder();
		foreach (var t in hash)
		{
			builder.Append(t.ToString("x2"));
		}

		return builder.ToString();
	}

	private static char[] constant = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};

	/// <summary>
	/// 生成0-9的随机字符串
	/// </summary>
	public static string GenerateRnd(int length)
	{
		var checkCode = String.Empty;
		var rd = new System.Random();
		for (var i = 0; i < length; i++)
		{
			checkCode += constant[rd.Next(10)].ToString();
		}
		return checkCode;
	}

	public static string GetTimeStamp()
	{
		var ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
		return Convert.ToInt64(ts.TotalSeconds).ToString();
	}

	/// <summary>
	/// {
	///		"code":0,
	///		"data":{
	///				"expires":3597,
	///				"uploadToken":"erg4OyxMUZtAYv5F1kXq3NAsKsxFTZTF"
	///		  		},
	///		"message":null
	/// }
	/// </summary>
	[Serializable]
	class TokenData
	{
		public int code = 0;
		public TokenItemData data;
		public string message;

		public static TokenData Parse(string res)
		{
			TokenData resultData = null;
			try
			{
				resultData = JsonUtility.FromJson<TokenData>(res);
			}
			catch (Exception e)
			{
				PTDebug.LogError(string.Format("Type:{0} 解析出错,解析数据:{1}", "TokenData", res));
				return null;
			}
			return resultData;
		}

		public static string GetUploadToken(string res)
		{
			return Parse(res).data.uploadToken;
		}
	}

	[Serializable]
	class TokenItemData
	{
		public int expires;
		public string uploadToken;
	}

//		"code":0,
//		"data":{
//			"ext":"jpg",
//			"filename":"1c6668fb7923b23d28334667a71323a3e1be8965.jpg",
//			"hash":"25A1BAFB348040135D0DFFF2C2803339",
//			"heigth":0,
//			"width":0
//		},
//		"message":null
	[Serializable]
	class RespUploadData
	{
		public int code = 0;
		public RespUploadItemData data;
		public string message;

		public static RespUploadData Parse(string res)
		{
			RespUploadData resultData = null;
			try
			{
				resultData = JsonUtility.FromJson<RespUploadData>(res);
			}
			catch (Exception e)
			{
				PTDebug.LogError(string.Format("Type:{0} 解析出错,解析数据:{1}", "RespUploadData", res));
				return null;
			}
			return resultData;
		}

		public static string GetFileName(string res)
		{
			if (res.IsNullOrEmpty())
				return null;
			var data = Parse(res);
			if (data == null)
				return null;
			return data.data.filename;
		}
	}

	[Serializable]
	class RespUploadItemData
	{
		public string ext;
		public string filename;
		public string hash;
		public int height;
		public int width;
	}

}

public class PostData
{
	public string Url;
	public WWWForm Form;

	public PostData SetUrl(string url)
	{
		Url = url;
		return this;
	}
		
	public PostData SetForm(WWWForm form)
	{
		Form = form;
		return this;
	}
}