/****************************************************************************
 * Copyright (c) 2018 maoling@putao.com
 ****************************************************************************/

using System;
using UnityEngine;

[Serializable]
public class PBCircle
{
    public Transform transform;
    public float radius;

    public PBCircle(Transform _trans, float _radius)
    {
        transform = _trans;
        radius = _radius;
    }

    public Vector3 center
    {
        get { return transform.position; }
    }

    public Vector3 forward
    {
        get { return transform.forward; }
    }

    public Vector3 right
    {
        get { return transform.right; }
    }

    public Vector3 up
    {
        get { return transform.up; }
    }
}

public class PBCircleNode
{
    public PBCircle circle;

    public Vector3 position;
    public Vector3 tangent;    //切线单位向量
    public float angle;    //与circle.up的夹脚（0，360），逆时针

    public float distance;    //距离起点的长度

    public PBCircleNode(PBCircle _circle, Vector3 _position, Vector3 _tangent, float _angle)
    {
        circle = _circle;
        position = _position;
        tangent = _tangent;
        angle = _angle;
    }

    public PBCircleNode(PBCircleNode node)
    {
        circle = node.circle;
        position = node.position;
        tangent = node.tangent;
        angle = node.angle;
    }

    /// <summary>
    /// 两点间连直线（两圆公切线）
    /// </summary>
    public bool ConnectLine(PBCircleNode nextNode)
    {
        return nextNode.circle != this.circle;
    }

    /// <summary>
    /// 两点间连圆弧
    /// </summary>
    public bool ConnectArc(PBCircleNode nextNode)
    {
        return nextNode.circle == this.circle;
    }

    public float deltaAngle(PBCircleNode targetNode)
    {
        float deltaAngle = targetNode.angle - this.angle;
        if (deltaAngle < 0)
            deltaAngle += 360;
        return deltaAngle;
    }
}
