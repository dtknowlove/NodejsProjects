
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class RefPointMgr
{
    public class RefPointGroup
    {
        public readonly RefPointType type;
        public readonly RefPointType[] pairs;
        public readonly Color color;

        public RefPointGroup(RefPointType type)
        {
            this.type = type;

            switch (type)
            {
                case RefPointType.male:
                {
                    pairs = new[] {RefPointType.female};
                    color = new Color(0, 0, 1);
                    break;
                }
                case RefPointType.male71:
                {
                    pairs = new[] {RefPointType.female71};
                    color = new Color(0, 1, 0);
                    break;
                }
                case RefPointType.male34:
                {
                    pairs = new[] {RefPointType.female34, RefPointType.claw34, RefPointType.cross50};
                    color = new Color(0, 1, 1);
                    break;
                }
                case RefPointType.axis34:
                {
                    pairs = new[] {RefPointType.female34, RefPointType.claw34, RefPointType.cross50};
                    color = new Color(1, 1, 0);
                    break;
                }
                case RefPointType.axis50:
                {
                    pairs = new[] {RefPointType.hole50, RefPointType.cross50,RefPointType.claw50};
                    color = new Color(1, 0, 1);
                    break;
                }
                case RefPointType.male50:
                {
                    pairs = new[] {RefPointType.female50, RefPointType.hole50};
                    color = new Color(1, 0, 0);
                    break;
                }
            }
        }
    }

    private static List<RefPointGroup> refGroups = new List<RefPointGroup>
    {
        new RefPointGroup(RefPointType.male),
        new RefPointGroup(RefPointType.male71),
        new RefPointGroup(RefPointType.male34),
        new RefPointGroup(RefPointType.axis34),
        new RefPointGroup(RefPointType.axis50),
        new RefPointGroup(RefPointType.male50),
    };

    public static void SetActive(RefPointGroup group, bool active)
    {
        EditorPrefs.SetBool(string.Format("RefPoint_{0}_active", group.type), active);
    }

    public static bool IsActive(RefPointGroup group)
    {
        return EditorPrefs.GetBool(string.Format("RefPoint_{0}_active", group.type), true);
    }

    public static bool IsActive(RefPointType type)
    {
        foreach (RefPointGroup group in refGroups)
        {
            if (group.type == type)
                return IsActive(group);
            foreach (RefPointType pair in group.pairs)
            {
                if (pair == type)
                {
                    if (IsActive(group))
                        return true;
                    break;
                }
            }
        }
        return false;
    }

    public static void LayoutToggles()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(30);
        GUILayout.BeginVertical();
        for (int i = 0; i < refGroups.Count; i++)
        {
            GUILayout.BeginHorizontal();

            RefPointGroup group = refGroups[i];
            bool isActiveOld = IsActive(group);
            bool isActive = EditorGUILayout.ToggleLeft("", isActiveOld, GUILayout.Width(20));
            if (isActive != isActiveOld)
            {
                SetActive(group, isActive);
            }
            GUI.color = group.color;
            EditorGUILayout.LabelField(group.type.ToString() + " ", GUILayout.Width(60));
            foreach (RefPointType type in group.pairs)
            {
                EditorGUILayout.LabelField(type.ToString() + " ", GUILayout.Width(60));
            }
            GUI.color = Color.white;
            
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

    public static RefPointType[] GetGroupOfType(RefPointType refType)
    {
        List<RefPointType> types = new List<RefPointType>();
        types.Add(refType);

        foreach (RefPointGroup group in refGroups)
        {
            if (refType == group.type)
            {
                types.AddRange(group.pairs);
                return types.ToArray();
            }
        }

        foreach (RefPointGroup group in refGroups)
        {
            foreach (RefPointType type in group.pairs)
            {
                if (refType == type)
                {
                    types.Add(group.type);
                    break;
                }
            }
        }
        return types.ToArray();
    }

    public static bool EnablePair(RefPointType refType1, RefPointType refType2)
    {
        foreach (RefPointGroup group in refGroups)
        {
            if (refType1 == group.type)
            {
                foreach (RefPointType type in group.pairs)
                {
                    if (refType2 == type)
                        return true;
                }
            }
            else if (refType2 == group.type)
            {
                foreach (RefPointType type in group.pairs)
                {
                    if (refType1 == type)
                        return true;
                }
            }
        }
        return false;
    }

    public static GameObject GetRefPointGameObject(RefPointType refType)
    {
        GameObject prefab = Resources.Load<GameObject>("RefPoint/refpoint");
        GameObject go = GameObject.Instantiate(prefab);
        go.name = "refpoint";

        string matName = "";
        Color color = Color.white;
        foreach (RefPointGroup group in refGroups)
        {
            if (group.type == refType)
            {
                matName = group.type.ToString().ToLower();
                color = group.color;
                break;
            }
        }
        foreach (RefPointGroup group in refGroups)
        {
            foreach (RefPointType type in group.pairs)
            {
                if (type == refType)
                {
                    matName = group.type.ToString().ToLower();
                    color = group.color;
                    break;
                }
            }
        }

        string matPath = "Assets/BlockEditor/Resources/Refpoint/Materials/" + matName + ".mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
        if (mat == null)
        {
            mat = new Material(Shader.Find("Standard"));
            mat.color = color;
            AssetDatabase.CreateAsset(mat, matPath);
            AssetDatabase.Refresh();

            mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
        }

        Renderer renderer = go.GetComponent<Renderer>();
        renderer.sharedMaterial = mat;
        return go;
    }
}
#endif