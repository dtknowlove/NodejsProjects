/****************************************************************************
 * Copyright (c) 2017 liuzhenhua@putao.com
 ****************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Putao.PaiBloks.Common
{
    public static class MonoExtension
    {
        public static void InvokeSafely(this Action action)
        {
            if (null != action)
            {
                action.Invoke();
            }
        }

        public static bool InvokeSafely<T>(this Action<T> selfAction, T t)
        {
            if (null != selfAction)
            {
                selfAction(t);
                return true;
            }
            return false;
        }

        public static T GetCom<T>(this GameObject obj) where T : Component
        {
            var res = obj.GetComponent<T>();
            if (null == res)
            {
                res = obj.AddComponent<T>();
            }
            return res;
        }

        public static void RemoveCom<T>(this GameObject obj) where T : Component
        {
            var res = obj.GetComponent<T>();
            if (null == res)
                return;
            Object.Destroy(res);
        }

        public static Vector3 GetCenter(this Vector3 vet, Vector3 target)
        {
            return new Vector3(vet.x + target.x, vet.y + target.y, vet.z + target.z) / 2;
        }

        public static Transform SetLayer(this Transform selfTrans, int layer, bool isRecycle = true)
        {
            selfTrans.gameObject.layer = layer;
            if (isRecycle)
            {
                foreach (Transform child in selfTrans)
                {
                    child.SetLayer(layer, isRecycle);
                }
            }
            return selfTrans;
        }

        public static T DestroyAllChild<T>(this T selfComponent) where T : Component
        {
            var childCount = selfComponent.transform.childCount;

            for (var i = 0; i < childCount; i++)
            {
                selfComponent.transform.GetChild(i).DestroyGameObjGracefully();
            }

            return selfComponent;
        }

        public static void DestroyGameObjGracefully<T>(this T selfBehaviour) where T : Component
        {
            if (selfBehaviour && selfBehaviour.gameObject)
            {
                selfBehaviour.gameObject.DestroySelfGracefully();
            }
        }

        public static T DestroySelfGracefully<T>(this T selfObj) where T : Object
        {
            if (selfObj)
            {
                Object.Destroy(selfObj);
            }
            return selfObj;
        }

        public static Transform EulerAngleX(this Transform selfTrans, float x)
        {
            var angle = selfTrans.eulerAngles;
            angle.x = x;
            selfTrans.eulerAngles = angle;
            return selfTrans;
        }

        public static Transform EulerAngleY(this Transform selfTrans, float y)
        {
            var angle = selfTrans.eulerAngles;
            angle.y = y;
            selfTrans.eulerAngles = angle;
            return selfTrans;
        }

        public static Transform EulerAngleZ(this Transform selfTrans, float z)
        {
            var angle = selfTrans.eulerAngles;
            angle.z = z;
            selfTrans.eulerAngles = angle;
            return selfTrans;
        }

        public static Transform FindChildRecursively(this Transform selfTrans, string childName)
        {
            if (string.Equals(selfTrans.name, childName))
                return selfTrans;

            foreach (Transform child in selfTrans)
            {
                Transform found = FindChildRecursively(child, childName);
                if (found != null)
                    return found;
            }
            return null;
        }

#if !BLOCK_EDITOR

    public static bool HasMultiPartType(this List<PBPartInfo> selfPartInfos)
    {
        return !(selfPartInfos.TrueForAll(t =>
                     PPBlockDB.Instance.GetLargeParticlesType(t.PrefabName) == PartType.Large) ||
                 selfPartInfos.TrueForAll(t =>
                     PPBlockDB.Instance.GetLargeParticlesType(t.PrefabName) == PartType.Small) ||
                 selfPartInfos.TrueForAll(t =>
                     PPBlockDB.Instance.GetLargeParticlesType(t.PrefabName) == PartType.Sticker));
    }
    
    #endif
    }
}
