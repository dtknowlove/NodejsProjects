/****************************************************************************
 * Copyright (c) 2017 liuzhenhua@putao.com
 ****************************************************************************/

using System;
using System.Text;
using System.Xml;
using UnityEngine;

public static class XMLExtension 
{
    public static XmlElement SetVector3(this XmlElement xmlElement, string attrName, Vector3 pos)
    {
        xmlElement.SetAttribute(attrName + "x", pos.x.ToString());
        xmlElement.SetAttribute(attrName + "y", pos.y.ToString());
        xmlElement.SetAttribute(attrName + "z", pos.z.ToString());
        return xmlElement;
    }

    public static XmlNode GetVector3(this XmlNode xmlNode, string attrName, out Vector3 pos)
    {
        float posx = float.Parse(xmlNode.Attributes[attrName + "x"].Value);
        float posy = float.Parse(xmlNode.Attributes[attrName + "y"].Value);
        float posz = float.Parse(xmlNode.Attributes[attrName + "z"].Value);
        pos = new Vector3(posx, posy, posz);
        return xmlNode;
    }
    
    public static XmlElement SetVector4(this XmlElement xmlElement, string attrName, Vector4 pos)
    {
        xmlElement.SetAttribute(attrName + "x", pos.x.ToString());
        xmlElement.SetAttribute(attrName + "y", pos.y.ToString());
        xmlElement.SetAttribute(attrName + "z", pos.z.ToString());
        xmlElement.SetAttribute(attrName + "w", pos.w.ToString());
        return xmlElement;
    }

    public static XmlNode GetVector4(this XmlNode xmlNode, string attrName, out Vector4 pos)
    {
        float posx = float.Parse(xmlNode.Attributes[attrName + "x"].Value);
        float posy = float.Parse(xmlNode.Attributes[attrName + "y"].Value);
        float posz = float.Parse(xmlNode.Attributes[attrName + "z"].Value);
        float posw = float.Parse(xmlNode.Attributes[attrName + "w"].Value);
        pos = new Vector4(posx, posy, posz, posw);
        return xmlNode;
    }

    public static XmlElement SetPosInfo(this XmlElement xmlElement, Vector3 pos)
    {
        return xmlElement.SetVector3("pos", pos);
    }

    public static XmlElement SetAngleInfo(this XmlElement xmlElement, Vector3 angle)
    {
        return xmlElement.SetVector3("angle", angle);
    }

    public static XmlNode GetPosInfo(this XmlNode xmlNode, out Vector3 pos)
    {
        return xmlNode.GetVector3("pos", out pos);
    }

    public static XmlNode GetAngleInfo(this XmlNode xmlNode, out Vector3 angle)
    {
        return xmlNode.GetVector3("angle", out angle);
    }

    public static XmlElement SetFloatArray(this XmlElement xmlElement, string attrName, float[] floatArr)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < floatArr.Length; i++)
        {
            sb.Append(floatArr[i] + ";");
        }
        xmlElement.SetAttribute(attrName, sb.ToString());
        return xmlElement;
    }

    public static XmlNode GetFloatArray(this XmlNode xmlNode, string attrName, out float[] floatArr)
    {
        string[] strs = xmlNode.Attributes[attrName].Value.Split(new char[] {';'}, StringSplitOptions.RemoveEmptyEntries);
        floatArr = new float[strs.Length];
        for (int i = 0; i < strs.Length; i++)
        {
            floatArr[i] = float.Parse(strs[i]);
        }
        return xmlNode;
    }

    public static XmlElement SetStringArray(this XmlElement xmlElement, string attrName, string[] strArr)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < strArr.Length; i++)
        {
            sb.Append(strArr[i] + ";");
        }
        xmlElement.SetAttribute(attrName, sb.ToString());
        return xmlElement;
    }
    
    public static XmlNode GetStringArray(this XmlNode xmlNode, string attrName, out string[] strArr)
    {
        string[] strs = xmlNode.Attributes[attrName].Value.Split(new char[] {';'}, StringSplitOptions.RemoveEmptyEntries);
        strArr = new string[strs.Length];
        for (int i = 0; i < strs.Length; i++)
        {
            strArr[i] = strs[i];
        }
        return xmlNode;
    }
}
