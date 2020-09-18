/****************************************************************************
 * Copyright (c) 2018 maoling@putao.com
 ****************************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PBCircleCurve
{
    private PBCircleCurveType curveType;
    private LinkedList<PBCircleNode> nodes = new LinkedList<PBCircleNode>();

    public LinkedList<PBCircleNode> GetNodes()
    {
        return nodes;
    }

    public float Length
    {
        get { return nodes.Last.Value.distance; }
    }

    public bool IsBuilt
    {
        get { return nodes.Count > 0; }
    }

    public void Build(PBCircle[] circles, PBCircleCurveType type)
    {
        curveType = type;
        nodes = PBCircleCurveBuilder.Build(circles, type);
    }

    public void Clear()
    {
        nodes.Clear();
    }

    /// <summary>
    /// 通过距离起点的距离，获取线上的点的坐标、斜率、圆心到坐标的单位向量（垂直于斜率)
    /// </summary>
    public void GetSampleAtDistance(float distance, out Vector3 position, out Vector3 tangent, out Vector3 emit)
    {
        LinkedListNode<PBCircleNode> listNode = nodes.First;
        PBCircleNode startNode = null;
        PBCircleNode endNode = null;
        while (listNode != null)
        {
            if (distance < listNode.Value.distance)
            {
                startNode = listNode.Previous.Value;
                endNode = listNode.Value;
                break;
            }
            listNode = listNode.Next;
        }

        if (startNode == null)
            throw new ArgumentException(string.Format("Distance must be less than curve length. Curve length: {0}, distance: {1}",
                nodes.Last.Value.distance, distance));

        float t = (distance - startNode.distance) / (endNode.distance - startNode.distance);
        if (startNode.ConnectLine(endNode))
        {
            //直线
            position = Vector3.Lerp(startNode.position, endNode.position, t);
            tangent = startNode.tangent;
            emit = (startNode.position - startNode.circle.center).normalized;
        }
        else
        {
            //圆弧
            PBCircle circle = startNode.circle;
            float angle = startNode.angle + t * startNode.deltaAngle(endNode);
            Vector3 r = Quaternion.AngleAxis(angle, circle.forward) * circle.up;
            r.Normalize();
            position = circle.center + r * circle.radius;

            tangent = Quaternion.AngleAxis(angle, circle.forward) * (-circle.right);

            emit = r;
        }
    }

    public void GetBottomMidSample(out Vector3 position, out Vector3 tangent, out Vector3 emit, out float distance)
    {
        //单圆
        if (curveType == PBCircleCurveType.OneCircle)
        {
            PBCircleNode node = nodes.First.Value;
            Vector3 r = -node.circle.up.normalized;
            position = node.circle.center + r * node.circle.radius;
            tangent = -node.tangent;
            emit = r;
            distance = 0.5f * nodes.Last.Value.distance;
            
            return;
        }

        PBCircleNode node1;
        PBCircleNode node2;
        if (curveType == PBCircleCurveType.Line)
        {
            int half = (nodes.Count - 1) / 2;
            int index = half + half / 2;
            node1 = nodes.ElementAt(index - 1);
            node2 = nodes.ElementAt(index);
        }
        else
        {
            int half = (nodes.Count - 1) / 2;
            node1 = nodes.ElementAt(half - 1);
            node2 = nodes.ElementAt(half);
        }

        if (node1.ConnectLine(node2))
        {
            position = Vector3.Lerp(node1.position, node2.position, 0.5f);
            tangent = node1.tangent;
            emit = (node1.position - node1.circle.center).normalized;
            distance = 0.5f * (node1.distance + node2.distance);
        }
        else
        {
            PBCircle circle = node1.circle;
            float angle = node1.angle + 0.5f * node1.deltaAngle(node2);
            Vector3 r = Quaternion.AngleAxis(angle, circle.forward) * circle.up;
            r.Normalize();
            position = circle.center + r * circle.radius;
            tangent = Quaternion.AngleAxis(angle, circle.forward) * (-circle.right);
            emit = r;
            distance = 0.5f * (node1.distance + node2.distance);
        }
    }
}