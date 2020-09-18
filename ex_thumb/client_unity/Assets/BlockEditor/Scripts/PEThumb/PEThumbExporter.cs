#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using PTGame.Core;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ThumbExportElement
{
	public string prefabPath;
	public Vector3 eulerAngle;
	public string modelName;
	public string material;
	public string materialH;
	public Category category;
	public string light;
	
	public PPPrefabInfo PrefabInfo;
	public PPBlockInfo BlockInfo;

	public string PrefabName
	{
		get { return BlockInfo.Prefab.IsNullOrEmpty() ? Path.GetFileNameWithoutExtension(prefabPath) : BlockInfo.Prefab; }
	}
}

[Serializable]
public class ThumbExportInfo
{
	public ThumbExportElement[] thumbs;
	public int scaleFactor = 4;
	public string saveFolder = "block_thumbs";
}

[Serializable]
public class UploadFiles
{
	public List<string> items;

	public UploadFiles()
	{
		items = new List<string>();
	}
}

public class PEThumbExporter : MonoBehaviour
{
	[SerializeField] private ThumbExportInfo m_ExportInfo;

	public void SetExportInfo(ThumbExportInfo exportInfo)
	{
		m_ExportInfo = exportInfo;
	}

	private const int SIZE = 240;
	
	private int SCREEN_SIZE;
	private Camera mCamera;
	private Transform transParent;
	private RenderTexture cameraRT;
	private Texture2D outTexture;
	private PEThumbLight thumbLight;
	private Dictionary<string,string> thumbFiles;
	private Action<UploadFiles,string> onFinish;
	private UploadFiles mUploadFiles;

	private void PrepareRender()
	{
		PPObjLoader.ReleaseAll();
		Transform parent = GameObject.Find("ThumbAnchor").transform;
		while (parent.childCount > 0)
		{
			GameObject.DestroyImmediate(parent.GetChild(0).gameObject);
		}

		mCamera = GetComponent<Camera>();
		Color cameraColor = mCamera.backgroundColor;
		cameraColor.a = 0;
		mCamera.backgroundColor = cameraColor;

		SCREEN_SIZE = m_ExportInfo.scaleFactor * SIZE;

		if (cameraRT != null && cameraRT.width != SCREEN_SIZE)
		{
			mCamera.targetTexture = null;
			cameraRT.Release();
			GameObject.DestroyImmediate(cameraRT);
			cameraRT = null;

			GameObject.DestroyImmediate(outTexture);
			outTexture = null;
		}

		if (cameraRT == null)
			cameraRT = new RenderTexture(SCREEN_SIZE, SCREEN_SIZE, 24, RenderTextureFormat.ARGB32);
		cameraRT.antiAliasing = 8;
		mCamera.targetTexture = cameraRT;

		if (outTexture == null)
			outTexture = new Texture2D(SCREEN_SIZE, SCREEN_SIZE, TextureFormat.RGBA32, false);

		transParent = GameObject.Find("ThumbAnchor").transform;
	}

	public void StartRender(Action<UploadFiles,string> callback = null)
	{
		thumbFiles = new Dictionary<string, string>();
		onFinish = callback;
		StartCoroutine(InnerStartRender());
	}

	private IEnumerator InnerStartRender()
	{
		if (m_ExportInfo == null || m_ExportInfo.thumbs == null || m_ExportInfo.thumbs.Length == 0)
		{
			onFinish.InvokeGracefully(null,"导入信息为空");
			yield break;
		}
		
		PrepareRender();
		
		PEThumbLight[] lights = GameObject.FindObjectsOfType<PEThumbLight>();
		for (int i = 0; i < lights.Length; i++)
			GameObject.DestroyImmediate(lights[i].gameObject);
		thumbLight = null;
		
		for (int i = 0; i < m_ExportInfo.thumbs.Length; i++)
		{
			var thumbExportInfo = m_ExportInfo.thumbs[i];
			string thumbName = thumbExportInfo.BlockInfo.Thumb.IsNotNullAndEmpty() ? thumbExportInfo.BlockInfo.Thumb : thumbExportInfo.PrefabName;
			
			var outPutPath = m_ExportInfo.saveFolder;
			if (!Directory.Exists(outPutPath))
			{
				Directory.CreateDirectory(outPutPath);
			}

			string thumbPath = Path.Combine(outPutPath, (thumbName + ".png").ToLower());
			
			Debug.LogFormat(">>>>> Export prefab_thumb {0} to thumb {1}", thumbName, thumbPath);
//			UnityEditor.EditorUtility.DisplayProgressBar("导出缩略图", prefabName, (float) i / m_ExportInfo.thumbs.Length);
			
			GameObject obj = null;
			var finish = false;
			PPObjLoader.LoadBlock(thumbExportInfo.PrefabInfo,thumbExportInfo.BlockInfo, transParent, o =>
			{
				finish = true;
				obj = o;
			});
			
			while (!finish)
			{
				yield return null;
			}
			if (obj == null)
				continue;
			yield return null;

			thumbPath.DeleteFileIfExists();
			Texture2D texture = RenderThumb(thumbExportInfo, obj, true);
			File.WriteAllBytes(thumbPath, texture.EncodeToPNG());
			if (!thumbFiles.ContainsKey(thumbPath))
				thumbFiles.Add(thumbPath, "");
		}
		PPObjLoader.ReleaseAll();
		yield return UploadFiles();
	}

	private int uploadCounter = 0;
	private string uploadMsg = string.Empty;
	private IEnumerator UploadFiles()
	{
		RenderThumbMgr.ReportSegTime(5,"ToUpload");
		if (thumbFiles.Count == 0)
		{
			onFinish.InvokeGracefully(null, "待渲染零件个数为0");
			yield break;
		}
		uploadCounter = 0;
		mUploadFiles = new UploadFiles();
		uploadMsg = string.Empty;
		foreach (var file in thumbFiles.Keys)
		{
			UploadUtility.UploadFile(file, OnUploadItemFinish);
			yield return null;
		}
	}

	private void OnUploadItemFinish(string file,string md5png)
	{
		uploadCounter++;
		var success = !string.IsNullOrEmpty(md5png);
		if (success)
		{
			var url = UploadUtility.GetDownloadFileUrl(md5png);
			Debug.Log("===上传完成>>" + url);
			if (thumbFiles.ContainsKey(file))
			{
				thumbFiles[file] = url;
			}
		}
		else
		{
			uploadMsg = string.Format("上传至文件服务器失败！名称:{0}", Path.GetFileName(file));
		}
		
		if (uploadCounter >= thumbFiles.Count)
		{
			thumbFiles.Values.ForEach(t =>
			{
				if (t.IsNotNullAndEmpty())
					mUploadFiles.items.Add(t);
			});
			onFinish.InvokeGracefully(mUploadFiles, uploadMsg);
		}
	}

	public void StartPreview()
	{
		StartCoroutine(InnerStartPreview());
	}

	private IEnumerator InnerStartPreview()
	{
		if (m_ExportInfo == null || m_ExportInfo.thumbs == null || m_ExportInfo.thumbs.Length != 1)
			yield break;

		PrepareRender();
		
		PEThumbLight[] lights = GameObject.FindObjectsOfType<PEThumbLight>();
		for (int i = 1; i < lights.Length; i++)
			GameObject.DestroyImmediate(lights[i].gameObject);
		thumbLight = lights.Length > 0 ? lights[0] : null;

		GameObject obj = null;
		var finish = false;
		var exportInfo = m_ExportInfo.thumbs[0];
		PPObjLoader.LoadBlock(exportInfo.PrefabInfo,exportInfo.BlockInfo, transParent, o =>
		{
			finish = true;
			obj = o;
		});

		while (!finish)
		{
			yield return null;
		}
		if (obj == null)
			yield break;
		yield return null;

		Texture2D texture = RenderThumb(m_ExportInfo.thumbs[0], obj, false);
		RawImage previewImage = FindObjectOfType<RawImage>();
		previewImage.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, SCREEN_SIZE);
		previewImage.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, SCREEN_SIZE);
		previewImage.texture = texture;
	}

	private Texture2D RenderThumb(ThumbExportElement thumbExportInfo, GameObject obj, bool destroyObj)
	{
		var prefabName = thumbExportInfo.PrefabName;

		if (obj == null)
			return null;
		
		if (thumbLight == null || thumbLight.lightName != thumbExportInfo.light)
			thumbLight = PEThumbLighting.LoadForColor(thumbExportInfo.light).GetComponent<PEThumbLight>();

		obj.transform.SetParent(transParent, false);
		obj.name = prefabName;
		obj.transform.localEulerAngles = thumbExportInfo.eulerAngle;

		AdjustCamera(obj);

		mCamera.Render();

		RenderTexture.active = cameraRT;
		outTexture.ReadPixels(new Rect(0, 0, SCREEN_SIZE, SCREEN_SIZE), 0, 0);
		outTexture.Apply(false);
		RenderTexture.active = null;

		//修正alpha
		Color[] colors_opaque = outTexture.GetPixels();
		for (int j = 0; j < colors_opaque.Length; j++)
		{
			Color color = colors_opaque[j];
			colors_opaque[j] = color.a > 0 ? new Color(color.r, color.g, color.b, 1) : color;
		}
		outTexture.SetPixels(colors_opaque);
		outTexture.Apply(false);

		if (destroyObj)
			DestroyImmediate(obj);

		return outTexture;
	}


	private void AdjustCamera(GameObject go)
	{
		Bounds bounds = CalculateBounds(go);
		Rect viewRect = CalculateViewRect(bounds);
//		if (viewRect == Rect.zero)
//		{
//			mCamera.orthographicSize = (go.name.StartsWith("pbs") || go.name.StartsWith("fig")) ? 2.3f : 1f;
//			mCamera.transform.position = new Vector3(-17f, 12.15f, 0);
//			return;
//		}

		CalculateCameraPos(bounds, viewRect);
	}

	private Bounds CalculateBounds(GameObject go)
	{
		Renderer[] rList = go.GetComponentsInChildren<Renderer>(true);
		Bounds bounds = new Bounds();
		foreach (Renderer r in rList)
		{
			bounds.Encapsulate(r.bounds);
		}
		return bounds;
	}

	
	/*
		大颗粒缩略图生成规则：
		1*1的零件：缩略图宽度缩小到90pt，长度自适应。
		1*2的零件：宽度=136pt，长度自适应。
		1*3的零件：宽度=174pt，长度自适应。
		2*2的零件：宽度=153pt，长度自适应。
		2*3的零件：宽度=198pt，长度自适应。
		以上尺寸的零件，若当宽度=固定值时，长度超过了220pt，即按长度=220pt，宽度自适应进行等比缩放。
		大于以上尺寸的零件，按最长边=220pt，进行等比缩放。
		
		小颗粒缩放规则：
		在大颗粒的尺寸基础上*0.9，最大边长=198pt
		
		轴的缩放规则：
		总体原则是：不同长度的轴，宽度不变，长度进行等比缩放。
		一个单位零件窗的轴：最长的轴长度=306pt，宽度=7pt；其他轴的长度等比裁剪，宽度不变=7pt。
		两个单位零件窗的轴：最长的轴长度=480pt，宽度=10pt；其他轴的长度等比裁剪，宽度不变=10pt。
		
		公仔件的缩放规则：
		同小颗粒缩放规则
	*/
	private Rect CalculateViewRect(Bounds bounds)
	{
		float viewWidth = 0;
		float viewHeight = 0;
		viewWidth = 220;
		
		float angle = Mathf.Deg2Rad * mCamera.transform.eulerAngles.x;
		Vector3 boundDelta = bounds.max - bounds.min;
		float boundWidth = Mathf.Abs(boundDelta.z);
		float boundHeight = Mathf.Abs(boundDelta.y);
		float boundDepth = Mathf.Abs(boundDelta.x);
		float boundAngle = Mathf.Atan(boundHeight / boundDepth);
		float boundViewHeight = boundHeight / Mathf.Sin(boundAngle) * Mathf.Sin(boundAngle + angle);
		float ratio = boundViewHeight / boundWidth;	
		
		viewHeight = ratio * viewWidth;
		if (viewHeight > 220)
		{
			viewHeight = 220;
			viewWidth = viewHeight / ratio;
		}

		float xMin, width, yMin, height;
		width = viewWidth / 240;
		height = viewHeight / 240;
		

		xMin = (1 - width) * 0.5f;
		yMin = (1 - height) * 0.5f;
		
		return new Rect(xMin, yMin, width, height);
	}
	
	private void CalculateCameraPos(Bounds bounds, Rect viewRect)
	{
		//Debug.LogFormat(">>>>> ViewRect: ({0}, {1}, {2}, {3})", viewRect.xMin, viewRect.yMin, viewRect.width, viewRect.height);
		
		float angle = Mathf.Deg2Rad * mCamera.transform.eulerAngles.x;
		float dist = 20;

		Vector3 boundDelta = bounds.max - bounds.min;
		float boundWidth = Mathf.Abs(boundDelta.z);
		float boundHeight = Mathf.Abs(boundDelta.y);
		float boundDepth = Mathf.Abs(boundDelta.x);
		float boundAngle = Mathf.Atan(boundHeight / boundDepth);
		
		float boundViewHeight = boundHeight / Mathf.Sin(boundAngle) * Mathf.Sin(boundAngle + angle);

		//calculate the orthographicSize
		float viewHeight = boundViewHeight / viewRect.height;
		float viewWidth = viewHeight * mCamera.aspect;

		//The orthographicSize is half the size of the vertical viewing volume
		mCamera.orthographicSize = viewHeight * 0.5f;
		
		Vector3 resPos = bounds.center - new Vector3(0, boundHeight * 0.5f, 0);

		//x
		float x = -dist;
			
		//y
		float deltaD = viewHeight * (viewRect.yMax - 0.5f);
		
		float deltaX = deltaD * Mathf.Sin(angle);
		float deltaY = deltaD * Mathf.Cos(angle);
		float y = (dist - 0.5f * boundDepth + deltaX) * Mathf.Tan(angle) + deltaY;
			
		//z
		float z = -(viewRect.center.x - 0.5f) * viewWidth;

		resPos += new Vector3(x, y, z);

		mCamera.transform.position = resPos;
	}
}

#endif