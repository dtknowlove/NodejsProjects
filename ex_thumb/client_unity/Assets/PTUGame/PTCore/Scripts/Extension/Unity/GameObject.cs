/****************************************************************************
 * Copyright (c) 2017 ptgame@putao.com
 ****************************************************************************/



namespace PTGame.Core
{
    using UnityEngine;

    /// <summary>
    /// GameObject's Util/Static This Extension
    /// </summary>
    public static class GameObjectExtension
    {
        #region CEGO001 Show

        public static GameObject Show(this GameObject selfObj)
        {
            selfObj.SetActive(true);
            return selfObj;
        }

        public static T Show<T>(this T selfComponent) where T : Component
        {
            selfComponent.gameObject.Show();
            return selfComponent;
        }

        public static GameObject ShowGracefully(this GameObject selfGameObj)
        {
            if (selfGameObj) selfGameObj.Show();
            return selfGameObj;
        }

        public static T ShowGracefully<T>(this T selfComponent) where T : Component
        {
            if (selfComponent) selfComponent.Show();
            return selfComponent;
        }
        

        #endregion

        #region CEGO002 Hide

        public static GameObject Hide(this GameObject selfObj)
        {
            selfObj.SetActive(false);
            return selfObj;
        }

        public static T Hide<T>(this T selfComponent) where T : Component
        {
            selfComponent.gameObject.Hide();
            return selfComponent;
        }

        public static GameObject HideGracefully(this GameObject selfGameObj)
        {
            if (selfGameObj) selfGameObj.Hide();
            return selfGameObj;
        }

        public static T HideGracefully<T>(this T selfComponent) where T : Component
        {
            if (selfComponent) selfComponent.Hide();
            return selfComponent;
        }

        #endregion

        #region CEGO003 DestroyGameObj

        public static void DestroyGameObj<T>(this T selfBehaviour) where T : Component
        {
            selfBehaviour.gameObject.DestroySelf();
        }

        #endregion

        #region CEGO004 DestroyGameObjGracefully

        public static void DestroyGameObjGracefully<T>(this T selfBehaviour, bool immediate = false) where T : Component
        {
            if (selfBehaviour && selfBehaviour.gameObject)
            {
                if (immediate) GameObject.DestroyImmediate(selfBehaviour.gameObject);
                else GameObject.Destroy(selfBehaviour.gameObject);
            }
        }

        #endregion

        #region CEGO005 DestroyGameObjGracefully

        public static T DestroyGameObjAfterDelay<T>(this T selfBehaviour, float delay) where T : Component
        {
            selfBehaviour.gameObject.DestroySelfAfterDelay(delay);
            return selfBehaviour;
        }

        public static T DestroyGameObjAfterDelayGracefully<T>(this T selfBehaviour, float delay) where T : Component
        {
            if (selfBehaviour && selfBehaviour.gameObject)
            {
                selfBehaviour.gameObject.DestroySelfAfterDelay(delay);
            }
            return selfBehaviour;
        }

        #endregion

        #region CEGO006 Layer

        public static GameObject Layer(this GameObject selfObj, int layer, bool recursively = false)
        {
            selfObj.layer = layer;
            if (recursively)
            {
                foreach (Transform child in selfObj.transform)
                    child.gameObject.Layer(layer, true);
            }

            return selfObj;
        }

        public static T Layer<T>(this T selfComponent, int layer, bool recursively = false) where T : Component
        {
            selfComponent.gameObject.Layer(layer, recursively);
            return selfComponent;
        }

        public static GameObject Layer(this GameObject selfObj, string layerName, bool recursively = false)
        {
            selfObj.Layer(LayerMask.NameToLayer(layerName), recursively);
            return selfObj;
        }

        public static T Layer<T>(this T selfComponent, string layerName, bool recursively = false) where T : Component
        {
            selfComponent.gameObject.Layer(layerName, recursively);
            return selfComponent;
        }

        #endregion
        
        
        public static T AddSingleComponent<T> (this GameObject gameObject) where T:Component
        {
            T com = gameObject.GetComponent<T> ();
            if (com == null) {
                com = gameObject.AddComponent<T> ();
            }
            return com;
        }
        public static T GetSingleComponent<T> (this GameObject gameObject) where T:Component
        {
            T com = gameObject.GetComponent<T> ();
            if (com == null) {
                com = gameObject.AddComponent<T> ();
            }
            return com;
        }
        
        public static void RemoveCom<T>(this GameObject obj) where T : Component
        {
            var res = obj.GetComponent<T>();
            if(null==res)
                return;
            Object.Destroy(res);
        }
        
    }
}    