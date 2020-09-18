/****************************************************************************
 * Copyright (c) 2018 maoling@putao.com
 ****************************************************************************/

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PBSplineTrack))]
public class PBSplineTrackEditor : Editor
{
    private PBSplineTrack splineTrack;
    private PBSplineTrackWindow window;

    private void OnEnable()
    {
        splineTrack = target as PBSplineTrack;    
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        if (GUILayout.Button("Create"))
        {
            splineTrack.Curve.Clear();
            splineTrack.ForceUpdate();
            EditorUtility.SetDirty(target);
        }

        if (GUILayout.Button("Show Window"))
        {
            splineTrack.ShowWindow = true;
        }

        if (splineTrack.ShowWindow)
        {
            if (window == null)
            {
                Debug.Log(">>>>>Show Spline Track Window");
                window = EditorWindow.GetWindow<PBSplineTrackWindow>(false, "履带编辑窗口", true);
                window.SetSplineTrack(splineTrack);
            }
        }
        else
        {
            if (Selection.activeGameObject == splineTrack.gameObject
                && Selection.gameObjects.Length == 1)
            {
                splineTrack.ShowWindow = true;
            }
            else
            {
                if (window != null)
                {
                    Debug.Log(">>>>>Close Spline Track Window");
                    window.Close();
                    window = null;
                }    
            }
        }
    }

    private void OnSceneGUI()
    {
        if (splineTrack.Curve == null || !splineTrack.Curve.IsBuilt)
            return;

        Color preColor = Handles.color;

        LinkedList<PBCircleNode> nodes = splineTrack.Curve.GetNodes();
        LinkedListNode<PBCircleNode> listNode = nodes.First.Next;
        PBCircleNode startNode;
        PBCircleNode endNode;
        while (listNode != null)
        {
            startNode = listNode.Previous.Value;
            endNode = listNode.Value;
            Handles.color = Color.green;
            if (startNode.ConnectLine(endNode))
            {
                Handles.DrawLine(startNode.position, endNode.position);
            }
            else
            {
                PBCircle circle = startNode.circle;
                Handles.DrawWireArc(circle.center, circle.forward, (startNode.position - circle.center).normalized, startNode.deltaAngle(endNode),
                    circle.radius);
            }

            Handles.color = Color.blue;
            Handles.SphereCap(0, startNode.position, Quaternion.identity, 0.05f);
            listNode = listNode.Next;
        }

        Handles.color = preColor;
    }
}