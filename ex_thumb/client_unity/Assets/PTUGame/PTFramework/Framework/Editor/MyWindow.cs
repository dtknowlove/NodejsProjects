/****************************************************************************
 * Copyright (c) 2017 dongwanlin@putao.com
 ****************************************************************************/

using UnityEditor;
using UnityEngine;
using System;

//http://www.jianshu.com/p/68e3445a421b
class MyWindow : EditorWindow
{
	static string[] text;
	[MenuItem("Window/My Window")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(MyWindow));
		var str = Resources.Load<TextAsset> ("t").text;
		text = str.Split ('\n');
	}
	public Vector2 scrollPosition;
	void OnGUI()
	{
		scrollPosition = GUILayout.BeginScrollView(scrollPosition);
		//鼠标放在按钮上的样式
		foreach (MouseCursor item in Enum.GetValues(typeof(MouseCursor)))
		{
			GUILayout.Button(Enum.GetName(typeof(MouseCursor), item));
			EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), item);
			GUILayout.Space(10);
		}

		if(text == null)
		{
			var str = Resources.Load<TextAsset> ("t").text;
			text = str.Split ('\n');
		}
		//内置图标
		for(var i =0; i< text.Length; i+=8)
		{
			GUILayout.BeginHorizontal();
			for (var j =0; j < 8; j++)
			{
				var index = i + j;
				if (index < text.Length) 
				{
					if(GUILayout.Button(EditorGUIUtility.IconContent(text[index]), GUILayout.Width(50), GUILayout.Height(30)))
					{
						Debug.Log ("name:"+text[index]);
					}
				}
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndScrollView();
	}
}
