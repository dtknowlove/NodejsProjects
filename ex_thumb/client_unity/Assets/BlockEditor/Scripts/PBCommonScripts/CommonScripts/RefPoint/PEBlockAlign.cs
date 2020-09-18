using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PEBlockAlign : MonoBehaviour
{
    public void SetRefPointTypes()
    {
        #if UNITY_EDITOR
        foreach (Transform child in transform)
        {
            RefPointType refType;
            if (GetRefPointType(child.name, out refType))
            {
                child.gameObject.AddComponent<RefDot>().Init(this,refType);
            }
        }
        #endif
    }

    private bool GetRefPointType(string objName, out RefPointType refType)
    {
        refType = RefPointType.male;
        if (!objName.StartsWith("ref_"))
            return false;

        string[] strs = objName.Split(new char[] {'_'}, StringSplitOptions.RemoveEmptyEntries);
        if (strs.Length < 3)
            return false;

        string refStr = strs[1].ToLower();
        foreach (RefPointType type in Enum.GetValues(typeof(RefPointType)))
        {
            if (refStr.Equals(type.ToString().ToLower()))
            {
                refType = type;
                return true;
            }
        }
        return false;
    }

    public void ShowRefPoints()
    {
        #if UNITY_EDITOR
        foreach (Transform child in transform)
        {
            RefPointType refType;
            if (!GetRefPointType(child.name, out refType))
                continue;
            
            child.localScale = Vector3.one;
            GameObject refpointObj = null;
            if (child.childCount == 0)
            {
                GameObject t = RefPointMgr.GetRefPointGameObject(refType);
                
                t.transform.SetParent(child, false);
                t.transform.localPosition = new Vector3(0, 0.102f, 0);
                t.transform.localRotation = Quaternion.identity;
                t.transform.localScale = Vector3.one;

                RefPoint refPoint = t.AddComponent<RefPoint>();
                refPoint.blockAlign = this;
                refPoint.refPointType = refType;

                refpointObj = t;
            }
            else
            {
                refpointObj = child.GetChild(0).gameObject;
            }
            refpointObj.SetActive(RefPointMgr.IsActive(refType));
        }
        #endif
    }

    public void ShowRefPointsOfTypes(params RefPointType[] refTypes)
    {
        ShowRefPoints();

        foreach (Transform child in transform)
        {
            RefPointType refType;
            if (!GetRefPointType(child.name, out refType))
                continue;

            child.GetChild(0).gameObject.SetActive(refTypes.Any(r => r == refType));
        }
    }

    public void ClearRefPoints()
    {
        foreach (Transform child in transform)
        {
            RefPointType refType;
            if (!GetRefPointType(child.name, out refType))
                continue;

            child.localScale = Vector3.one;
            if (child.childCount != 0)
            {
                DestroyImmediate(child.GetChild(0).gameObject);
            }
        }
    }

    public void HideRefPoints()
    {
        foreach (Transform child in transform)
        {
            RefPointType refType;
            if (!GetRefPointType(child.name, out refType))
                continue;

            if (child.childCount != 0)
            {
                child.GetChild(0).gameObject.SetActive(false);
            }
        }
    }
}