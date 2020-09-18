/****************************************************************************
 * Copyright (c) 2018 maoling@putao.com
 ****************************************************************************/

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PBSplineAnimator))]
public class PBSplineAnimatorEditor : Editor
{
    private PBSplineAnimator animator;
    private Spline spline;
    private SerializedProperty keyframes;

    private int setIndex = 0;
    private int modifyIndex = 0;
    private int highlightIndex = -1;

    private GUIStyle labelStyle;

    private void OnEnable()
    {
        animator = target as PBSplineAnimator;
        spline = animator.GetComponent<Spline>();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        GUI.color = Color.yellow;
        if (labelStyle == null)
        {
            labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = 14;
        }
        GUILayout.Label("请务必将初始状态添加为Keyframe_0 ！！！", labelStyle);
        GUI.color = Color.white;

        keyframes = serializedObject.FindProperty("Keyframes");
        for (int i = 0; i < keyframes.arraySize; i++)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("-", GUILayout.Width(30)))
            {
                animator.RemoveKeyframe(i);
                break;
            }
            if (i > 0)
            {
                if (GUILayout.Button("▲", GUILayout.Width(20)))
                {
                    animator.MoveKeyframe(i, i - 1);
                    highlightIndex = i - 1;
                }
            }
            if (i < keyframes.arraySize - 1)
            {
                if (GUILayout.Button("▼", GUILayout.Width(20)))
                {
                    animator.MoveKeyframe(i, i + 1);
                    highlightIndex = i + 1;
                }
            }

            if (i > 0 && i < keyframes.arraySize - 1)
            {
                GUILayout.Space(10);
            }
            else
            {
                GUILayout.Space(34);
            }
            
            GUILayout.BeginVertical();
            SerializedProperty keyframe = keyframes.GetArrayElementAtIndex(i);
            SerializedProperty keyframeNodes = keyframe.FindPropertyRelative("nodes");
            
            if (highlightIndex == i)
                GUI.color = Color.green;
            EditorGUILayout.PropertyField(keyframeNodes, new GUIContent("Keyframe_" + i));
            GUI.color = Color.white;
            
            EditorGUI.indentLevel += 1;
            if (keyframeNodes.isExpanded)
            {
                for (int j = 0; j < keyframeNodes.arraySize; j++)
                {
                    EditorGUILayout.PropertyField(keyframeNodes.GetArrayElementAtIndex(j), new GUIContent("Node_" + j), true);
                }
            }
            EditorGUI.indentLevel -= 1;
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        if (GUILayout.Button("+Keyframe"))
        {
            animator.AddKeyframe(spline.nodes);
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Reset To Keyframe"))
        {
            animator.SetKeyframe(setIndex);
            highlightIndex = setIndex;
        }
        setIndex = EditorGUILayout.IntField(setIndex, GUILayout.Width(40));
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Modify Keyframe"))
        {
            animator.ModifyKeyframe(spline.nodes, modifyIndex);
            highlightIndex = modifyIndex;
        }
        modifyIndex = EditorGUILayout.IntField(modifyIndex, GUILayout.Width(40));
        GUILayout.EndHorizontal();

        /*GUILayout.BeginHorizontal();
        if (GUILayout.Button("Play"))
        {
            animator.Play(playIndex, 0.5f, null);
        }
        playIndex = EditorGUILayout.IntField(playIndex, GUILayout.Width(40));
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Play All"))
        {
            animator.PlayAll(0.5f, null);
        }*/
    }
}
