/****************************************************************************
 * Copyright (c) 2018 maoling@putao.com
 ****************************************************************************/

using System.Collections.Generic;
using UnityEngine;

public enum PBCircleCurveType
{
    OneCircle,
    Line,
    Enclosed,
}

public class PBCircleCurveBuilder
{
    public static LinkedList<PBCircleNode> Build(PBCircle[] circles, PBCircleCurveType type)
    {
        LinkedList<PBCircleNode> nodes = null;
        switch (type)
        {
            case PBCircleCurveType.OneCircle:
                nodes = BuildForOne(circles);
                break;
            case PBCircleCurveType.Line:
                nodes = BuildForLine(circles);
                break;
            case PBCircleCurveType.Enclosed:
                nodes = BuildForEnclosed(circles);
                break;
        }
        
        //compute distance for nodes
        LinkedListNode<PBCircleNode> listNode = nodes.First;
        listNode.Value.distance = 0;
        listNode = listNode.Next;
        while (listNode != null)
        {
            PBCircleNode node = listNode.Value;
            PBCircleNode preNode = listNode.Previous.Value;
            if (node.ConnectLine(preNode))
            {
                //直线
                node.distance = preNode.distance + Vector3.Distance(node.position, preNode.position);
                //Debug.Log(">>>>>>>1 distance: " + node.distance);
            }
            else
            {
                //圆弧
                float radius = node.circle.radius;
                node.distance = preNode.distance + preNode.deltaAngle(node) * Mathf.Deg2Rad * radius;
                //Debug.Log(">>>>>>>2 distance: " + node.distance);
            }
            listNode = listNode.Next;
        }
        return nodes;
    }

    private static LinkedList<PBCircleNode> BuildForOne(PBCircle[] circles)
    {
        LinkedList<PBCircleNode> nodes = new LinkedList<PBCircleNode>();
        PBCircle circle = circles[0];

        PBCircleNode node1 = new PBCircleNode(
            circle,
            circle.center + circle.up * circle.radius,
            -circle.right,
            0);
        nodes.AddFirst(node1);

        PBCircleNode node2 = new PBCircleNode(node1);
        node2.angle = 360;
        nodes.AddLast(node2);

        return nodes;
    }

    private static LinkedList<PBCircleNode> BuildForLine(PBCircle[] circles)
    {
        LinkedList<PBCircleNode> nodes = new LinkedList<PBCircleNode>();

        for (int i = 0; i < circles.Length - 1; i++)
        {
            PBCircleNode[] subNodes = CalculateTangentLines(circles[i], circles[i + 1]);
            nodes.AddFirst(subNodes[0]);
            nodes.AddFirst(subNodes[1]);
            nodes.AddLast(subNodes[2]);
            nodes.AddLast(subNodes[3]);
        }
        
        //最后一个要跟第一个相连，所以再拷贝第一个点到末尾
        nodes.AddLast(new PBCircleNode(nodes.First.Value));
        
        /* for debug
        LinkedListNode<PBCircleNode> listNode = nodes.First;
        int index = 0;
        while (listNode != null)
        {
            PBCircleNode node = listNode.Value;
            Debug.Log(">>>>> " + index + "  " + node.angle);
            //DebugDraw.Ray(node.position, node.tangent, Color.cyan);
            listNode = listNode.Next;
            if (index == 0)
            {
                DebugDraw.Sphere(node.position, Color.red, 0.2f);
            }
            else if (index == 1)
            {
                DebugDraw.Sphere(node.position, Color.blue, 0.2f);
                
                
            }
            else if (index == 2)
                DebugDraw.Sphere(node.position, Color.yellow, 0.2f);
            else if (index == 3)
                DebugDraw.Sphere(node.position, Color.green, 0.2f);
            else 
                DebugDraw.Sphere(node.position, Color.white, 0.2f);
            
            index++;
        }*/
        
        return nodes;
    }

    private static LinkedList<PBCircleNode> BuildForEnclosed(PBCircle[] circles)
    {
        LinkedList<PBCircleNode> nodes = new LinkedList<PBCircleNode>();

        for (int i = 0; i < circles.Length; i++)
        {
            PBCircle c1 = circles[i];
            PBCircle c2 = i == circles.Length - 1 ? circles[0] : circles[i + 1];
            PBCircleNode[] subNodes = CalculateTangentLines(c1, c2);
            //只保留方向与圆心连线方向一致的
            //所以顺时针选择圆，就包围外圈，逆时针选择圆，就包围内圈
            nodes.AddLast(subNodes[2]);
            nodes.AddLast(subNodes[3]);
        }

        //最后一个要跟第一个相连，所以再拷贝第一个点到末尾
        nodes.AddLast(new PBCircleNode(nodes.First.Value));
        
        return nodes;
    }

    private static PBCircleNode[] CalculateTangentLines(PBCircle c1, PBCircle c2)
    {
        Vector3 o1o2 = c2.center - c1.center;
        float o1o2_distance = o1o2.magnitude;
        o1o2.Normalize();

        float angle;// 90-圆心连线与公切线的夹角
        if (Mathf.Approximately(c1.radius, c2.radius))
        {
            angle = 90;
        }
        else
        {
            float r1 = Mathf.Max(c1.radius, c2.radius);
            float r2 = Mathf.Min(c1.radius, c2.radius);

            float x = o1o2_distance * r1 / (r1 - r2);
            angle = Mathf.Acos(r1 / x) * Mathf.Rad2Deg;

            if (c1.radius < c2.radius)
                angle = 180 - angle;
        }

        Vector3[] Rs = new[]
        {
            Quaternion.AngleAxis(angle, c1.forward) * o1o2,
            Quaternion.AngleAxis(angle, -c1.forward) * o1o2,
        };

        PBCircleNode[] nodes = new PBCircleNode[4];

        for (int i = 0; i < Rs.Length; i++)
        {
            PBCircleNode n1, n2; //第 i 条公切线的c1、c2上的点

            Vector3 r = Rs[i];
            Vector3 p = c1.center + r * c1.radius;
            Vector3 tangent = Quaternion.AngleAxis(90, c1.forward) * r;
            n1 = new PBCircleNode(c1, p, tangent, GetAngle(c1, r));
            p = c2.center + r * c2.radius;
            n2 = new PBCircleNode(c2, p, tangent, GetAngle(c2, r));

            nodes[i * 2] = n1;
            nodes[i * 2 + 1] = n2;
        }
        return nodes;
    }

    private static float GetAngle(PBCircle circle, Vector3 dir)
    {
        float dot = Mathf.Clamp(Vector3.Dot(circle.up, dir), -1, 1);
        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
        Vector3 cross = Vector3.Cross(circle.up, dir);
        if (Vector3.Dot(cross, circle.forward) < 0)
            angle = -angle;
        if (angle < 0)
            angle += 360;
        
        return angle;
    }
}
