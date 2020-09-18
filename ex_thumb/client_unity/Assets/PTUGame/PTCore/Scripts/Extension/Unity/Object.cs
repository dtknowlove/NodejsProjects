/****************************************************************************
 * Copyright (c) 2017 ptgame@putao.com
 ****************************************************************************/


namespace PTGame.Core
{
    using UnityEngine;

    public static class ObjectExtension
    {
        #region CEUO001 Instantiate

        public static T Instantiate<T>(this T selfObj) where T : Object
        {
            return Object.Instantiate(selfObj);
        }

        #endregion

        #region CEUO002 Instantiate

        public static T Name<T>(this T selfObj, string name) where T : Object
        {
            selfObj.name = name;
            return selfObj;
        }

        #endregion

        #region CEUO003 Destroy Self

        public static void DestroySelf<T>(this T selfObj) where T : Object
        {
            Object.Destroy(selfObj);
        }

        public static T DestroySelfGracefully<T>(this T selfObj) where T : Object
        {
            if (selfObj)
            {
                Object.Destroy(selfObj);
            }
            return selfObj;
        }

        #endregion

        #region CEUO004 Destroy Self AfterDelay 

        public static T DestroySelfAfterDelay<T>(this T selfObj, float afterDelay) where T : Object
        {
            Object.Destroy(selfObj, afterDelay);
            return selfObj;
        }

        public static T DestroySelfAfterDelayGracefully<T>(this T selfObj, float delay) where T : Object
        {
            if (selfObj)
            {
                Object.Destroy(selfObj, delay);
            }
            return selfObj;
        }

        #endregion

        #region CEUO005 Apply Self To 

        public static T ApplySelfTo<T>(this T selfObj, System.Action<T> toFunction) where T : Object
        {
            toFunction.InvokeGracefully(selfObj);
            return selfObj;
        }

        public static T ApplySelfToGracefully<T>(this T selfObj, System.Action<T> toFunction) where T : Object
        {
            if (selfObj)
            {
                toFunction.InvokeGracefully(selfObj);
            }

            return selfObj;
        }

        #endregion

        #region CEUO006 DontDestroyOnLoad

        public static T DontDestroyOnLoad<T>(this T selfObj) where T : Object
        {
            Object.DontDestroyOnLoad(selfObj);
            return selfObj;
        }

        #endregion

        public static T As<T>(this Object selfObj) where T : Object
        {
            return selfObj as T;
        }

        public static T LogSelf<T>(this T selfObj)
        {
//            Log.I(selfObj);
            return selfObj;
        }
        
        public static T LogInfo<T>(this T selfObj, string msgContent, params object[] args) where T : Object
        {
//            Log.I(msgContent, args);
            return selfObj;
        }
    }
}