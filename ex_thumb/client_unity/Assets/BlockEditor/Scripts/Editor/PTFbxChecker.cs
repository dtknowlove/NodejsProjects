using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class PTFbxChecker : MonoBehaviour {

	[MenuItem("Assets/PutaoTool/Check Fbx Format", false)]
	public static void ExportCheckList()
	{
		if (Selection.activeObject == null)
		{
			return;
		}

		if (!EditorUtility.IsPersistent(Selection.activeObject))
		{
			return;
		}
		string path = AssetDatabase.GetAssetPath(Selection.activeObject);
		
		List<string> fbxFiles = new List<string>();
		if (Directory.Exists(path))
		{
			string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
			fbxFiles.AddRange(files.Where(s => Path.GetExtension(s).ToLower() == ".fbx").ToList());
			if (fbxFiles.Count == 0)
			{
				EditorUtility.DisplayDialog("检测fbx", "该目录下没有fbx文件", "确定");
			}
			else
			{
				CheckFbx(fbxFiles);
			}
		}
		else
		{
			if (Path.GetExtension(path).ToLower() == ".fbx")
			{
				fbxFiles.Add(path);
				CheckFbx(fbxFiles);
			}
			else
			{
				EditorUtility.DisplayDialog("检测fbx", "请右键选择fbx文件，或者包含fbx文件的目录", "确定");
			}
		}
	}

	private static void CheckFbx(List<string> fbxFiles)
	{
		bool hasError = false;
		StringBuilder errorMsg = new StringBuilder();
		
		foreach (var fbxFile in fbxFiles)
		{
			GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(fbxFile);
			
			if (!V3Equal(obj.transform.localScale, Vector3.one))
			{
				hasError = true;
				errorMsg.AppendLine(obj.name+" 缩放值不为1");
			}
			
			if (!V3Equal(obj.transform.localEulerAngles,Vector3.zero))
			{
				hasError = true;
				errorMsg.AppendLine(obj.name+" 角度不为0");
			}
		}

		if (hasError)
		{
			Debug.LogError(errorMsg.ToString());
		}

		EditorUtility.DisplayDialog("检测完成", hasError?errorMsg.ToString():"全部正确", "确定");
	}
	public static bool V3Equal(Vector3 a, Vector3 b){
		return Vector3.SqrMagnitude(a - b) < 0.0001;
	}

}
