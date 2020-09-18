/****************************************************************************
 * Copyright (c) 2019 ptgame@putao.com
 ****************************************************************************/

namespace PTGame.Core
{
    using UnityEngine;

    public static class PathHelper
    {
        [RuntimeInitializeOnLoadMethod]
        public static void Init()
        {
            var init = PersistentDataPath;
        }
        
        public static string FileNameWithoutSuffix(this string name)
        {
            if (name == null)
            {
                return null;
            }

            var endIndex = name.LastIndexOf('.');
            if (endIndex > 0)
            {
                return name.Substring(0, endIndex);
            }

            return name;
        }

        public static string FullAssetPath2Name(this string fullPath)
        {
            var name = FileNameWithoutSuffix(fullPath);
            if (name == null)
            {
                return null;
            }

            var endIndex = name.LastIndexOf('/');
            return endIndex > 0 ? name.Substring(endIndex + 1) : name;
        }

        private static readonly string mCachedPersistentDataPath = UnityEngine.Application.persistentDataPath;

        public static string PersistentDataPath
        {
            get {
                return mCachedPersistentDataPath;
            }
        }
    }
}
