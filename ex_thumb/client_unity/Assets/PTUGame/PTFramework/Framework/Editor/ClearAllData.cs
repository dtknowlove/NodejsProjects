/****************************************************************************
 * Copyright (c) 2017 liqingyun@putao.com
 ****************************************************************************/

namespace PTGame.Framework.Editor
{
    using UnityEngine;
    using UnityEditor;
    using System.IO;

    public static class ClearAllData
    {   
        [MenuItem("PuTaoTool/Framework/ClearAllData")]
        private static void Clear()
        {
            PlayerPrefs.DeleteAll();
            Directory.Delete(Application.persistentDataPath, true);
            // quit the game
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }            
        }
    }
}