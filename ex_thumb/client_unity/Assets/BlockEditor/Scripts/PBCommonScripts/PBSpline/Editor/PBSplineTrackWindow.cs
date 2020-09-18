/****************************************************************************
 * Copyright (c) 2018 maoling@putao.com
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PBSplineTrackWindow : EditorWindow
{
    private PBSplineTrack splineTrack;
    private PBCircleCurveType curveType;
    private List<GameObject> circleObjects;

    private Array curveTypes;
    private Dictionary<PBCircleCurveType, GUIContent> guiContents;
    private GUIStyle labelStyle;
    private GUIStyle btnStyle;

    public void SetSplineTrack(PBSplineTrack track)
    {
        splineTrack = track;
        curveType = track.CurveType;

        circleObjects = new List<GameObject>();
        if (track.Circles != null && track.Circles.Length > 0)
        {
            foreach (PBCircle circle in track.Circles)
            {
                circleObjects.Add(circle.transform.gameObject);
            }
        }
    }

    private void OnEnable()
    {
        curveTypes = Enum.GetValues(typeof(PBCircleCurveType));
        
        guiContents = new Dictionary<PBCircleCurveType, GUIContent>();
        guiContents.Add(PBCircleCurveType.OneCircle, new GUIContent(
            AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/BlockEditor/Resources/Spline/1.png")));
        guiContents.Add(PBCircleCurveType.Line, new GUIContent(
            AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/BlockEditor/Resources/Spline/2.png")));
        guiContents.Add(PBCircleCurveType.Enclosed, new GUIContent(
            AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/BlockEditor/Resources/Spline/3.png")));

        labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.fontSize = 16;
        labelStyle.fontStyle = FontStyle.Normal;
        labelStyle.alignment = TextAnchor.MiddleLeft;
        
        btnStyle = new GUIStyle(GUI.skin.button);
        btnStyle.fontSize = 16;
        btnStyle.fontStyle = FontStyle.Normal;
        btnStyle.alignment = TextAnchor.MiddleCenter;
    }

    private void OnDestroy()
    {
        if (splineTrack != null)
            splineTrack.ShowWindow = false;
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        foreach (PBCircleCurveType type in curveTypes)
        {
            if (EditorGUILayout.ToggleLeft(guiContents[type], curveType == type, GUILayout.Width(150), GUILayout.Height(140))
                && curveType != type)
            {
                curveType = type;
            }    
            GUILayout.Space(10);
        }
        GUILayout.EndHorizontal();
        
        GUI.color = Color.red;
        GUILayout.Label("请按照顺序选择圆！！！", labelStyle);
        GUILayout.Label("顺序为：左手法则，逆时针选择！！！不懂请仔细看图，以及下面的温馨提示", labelStyle);
        GUI.color = Color.white;
        
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUI.color = Color.yellow;
        if (GUILayout.Button("已经选好圆\n我要创建履带", btnStyle, GUILayout.Width(200), GUILayout.Height(60)))
        {
            CreateTrack();
        }
        if (splineTrack != null)
        {
            GUILayout.Label("当前履带节数：" + splineTrack.ChildCount, labelStyle, GUILayout.Height(60));
            GUILayout.Space(10);
        }
        GUI.color = Color.white;
        GUILayout.EndHorizontal();
        
        GUILayout.Space(10);
        
        TuneGlobalSettings();
        
        GUILayout.Space(10);

        if (circleObjects != null)
        {
            foreach (GameObject circleObj in circleObjects)
            {
                EditorGUILayout.ObjectField("", circleObj, typeof(GameObject), GUILayout.Width(200));
            }    
        }

        GUILayout.Space(10);

        GUILayout.Label("温馨提示", labelStyle);
        GUILayout.Label("1. 确保所有圆在同一平面，并且forward方向一致！！！", labelStyle);
        GUILayout.Label("2. 使用左手法则：大拇指朝着forward方向，确定其他手指旋转的方向（逆时针/顺时针", labelStyle);
        GUILayout.Label("3. 确定了逆时针顺序，则按照此顺序选择圆", labelStyle);
        GUILayout.Label("4. 请尽量保证圆的up方向与所有圆组成的平面的up方向一致（可能会影响到平铺动画的效果）");
    }
    
    private void OnSelectionChange()
    {
        if (Selection.activeGameObject == null)
            return;
        
        if (Selection.gameObjects.Length == 1)
            circleObjects.Clear();
        
        circleObjects.Add(Selection.activeGameObject);
        circleObjects.RemoveAll(o => Selection.gameObjects.FirstOrDefault(go => go == o) == null);
        
        Debug.Log(">>>> OnSelectionChange " + Selection.activeGameObject.name);
    }

    private void CreateTrack()
    {
        if (splineTrack == null)
        {
            EditorUtility.DisplayDialog(null, "请把该窗口关闭，重新选择履带！", "OK");
            return;
        }

        if (circleObjects.Count == 0)
        {
            EditorUtility.DisplayDialog(null, "请选择圆！！！", "OK");
            return;
        }
        if (curveType == PBCircleCurveType.OneCircle && circleObjects.Count > 1)
        {
            EditorUtility.DisplayDialog(null, "单圆履带只能选择一个圆！！！", "OK");
            return;
        }
        if (curveType == PBCircleCurveType.Line && circleObjects.Count < 2)
        {
            EditorUtility.DisplayDialog(null, "排排站履带要选择至少两个圆！！！", "OK");
            return;
        }
        if (curveType == PBCircleCurveType.Enclosed && circleObjects.Count < 3)
        {
            EditorUtility.DisplayDialog(null, "包围圈履带要选择至少三个圆！！！", "OK");
            return;
        }

        Vector3 normal = circleObjects[0].transform.forward;
        for (int i = 1; i < circleObjects.Count; i++)
        {
            if (circleObjects[i].transform.forward != normal)
            {
                EditorUtility.DisplayDialog(null, "所有的圆必须要在同一个平面，forward方向保持一致！！！", "OK");
                return;
            }
        }

        PBCircle[] circles = new PBCircle[circleObjects.Count];
        for (int i = 0; i < circles.Length; i++)
        {
            float radius = 1f;
            if (circleObjects[i].name.Contains("40"))
                radius = Gear40Radius;
            else if (circleObjects[i].name.Contains("80"))
                radius = Gear80Radius;

            circles[i] = new PBCircle(circleObjects[i].transform, radius);
        }
        splineTrack.SetCircles(circles, curveType);
        EditorUtility.SetDirty(splineTrack);
    }

    private float Gear40Radius
    {
        get { return EditorPrefs.GetFloat("gear_40_radius", 0.5f); }
        set { EditorPrefs.SetFloat("gear_40_radius", value); }
    }

    private float Gear80Radius
    {
        get { return EditorPrefs.GetFloat("gear_80_radius", 1f); }
        set { EditorPrefs.SetFloat("gear_80_radius", value); }
    }

    private void TuneGlobalSettings()
    {
        if (splineTrack == null)
            return;

        bool update = false;

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("履带零件宽", GUILayout.Width(70));
        float newSpacing = EditorGUILayout.FloatField(splineTrack.Spacing, GUILayout.Width(40));
        if (!Mathf.Approximately(newSpacing, splineTrack.Spacing))
        {
            splineTrack.Spacing = newSpacing;
            update = true;
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("履带偏移", GUILayout.Width(70));
        bool leftOld = Mathf.Approximately(splineTrack.Deviation, -0.2f);
        bool left = EditorGUILayout.Toggle(leftOld, GUILayout.Width(15));
        if (left && !leftOld)
        {
            splineTrack.Deviation = -0.2f;
            update = true;
        }
        EditorGUILayout.LabelField("左", GUILayout.Width(30));

        bool centerOld = Mathf.Approximately(splineTrack.Deviation, 0);
        bool center = EditorGUILayout.Toggle(centerOld, GUILayout.Width(15));
        if (center && !centerOld)
        {
            splineTrack.Deviation = 0;
            update = true;
        }
        EditorGUILayout.LabelField("中", GUILayout.Width(30));

        bool rightOld = Mathf.Approximately(splineTrack.Deviation, 0.2f);
        bool right = EditorGUILayout.Toggle(rightOld, GUILayout.Width(15));
        if (right && !rightOld)
        {
            splineTrack.Deviation = 0.2f;
            update = true;
        }
        EditorGUILayout.LabelField("右", GUILayout.Width(30));

        GUILayout.EndHorizontal();

        if (update)
        {
            splineTrack.NeedUpdate();
            EditorUtility.SetDirty(splineTrack);
        }
        
        GUILayout.Space(10);
        EditorGUILayout.LabelField("以下参数修改后，需点击按钮重新创建履带", GUILayout.Width(500));
        
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("齿轮40半径", GUILayout.Width(70));
        Gear40Radius = EditorGUILayout.FloatField(Gear40Radius, GUILayout.Width(40));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("齿轮80半径", GUILayout.Width(70));
        Gear80Radius = EditorGUILayout.FloatField(Gear80Radius, GUILayout.Width(40));
        GUILayout.EndHorizontal();

        GUILayout.Space(10);
    }
}