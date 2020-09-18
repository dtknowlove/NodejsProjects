/****************************************************************************
 * Copyright (c) 2017 ptgame@putao.com
 ****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PTGame.Editor.PluginManager
{
    public class JsonUtilityHelper
    {
        //Usage:
        //YouObject[] objects = JsonHelper.getJsonArray<YouObject> (jsonString);
        public static T[] GetJsonArray<T>(string json)
        {
            string newJson = "{ \"array\": " + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.array;
        }
 
        //Usage:
        //string jsonString = JsonHelper.arrayToJson<YouObject>(objects);
        public static string ArrayToJson<T>(T[] array,bool pretty)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.array = array;
            return JsonUtility.ToJson(wrapper,pretty);
        }
 
        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] array;
        }
    }


}
