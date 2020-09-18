/****************************************************************************
* Copyright (c) 2019 ptgame@putao.com
****************************************************************************/

namespace PTGame.Core
{
	#if UNITY_EDITOR
	using UnityEditor;
	#endif
	using System;
	using System.IO;
	using System.Collections.Generic;

    public class EditorFileUtils
    {
        public static List<string> GetDirSubFilePathList(string dirABSPath, bool isRecursive = true, string suffix = "")
        {
            var pathList = new List<string>();
            var di = new DirectoryInfo(dirABSPath);

            if (!di.Exists)
            {
                return pathList;
            }
            UnityEngine.Debug.Log("lalalalal");
            var files = di.GetFiles();
            foreach (var fi in files)
            {
                if (!string.IsNullOrEmpty(suffix))
                {
                    if (!fi.FullName.EndsWith(suffix, System.StringComparison.CurrentCultureIgnoreCase))
                    {
                        continue;
                    }
                }
                UnityEngine.Debug.Log(fi.FullName);
                pathList.Add(fi.FullName);
            }

            if (isRecursive)
            {
                var dirs = di.GetDirectories();
                foreach (var d in dirs)
                {
                    pathList.AddRange(GetDirSubFilePathList(d.FullName, isRecursive, suffix));
                }
            }

            return pathList;
        }

        public static List<string> GetDirSubDirNameList(string dirABSPath)
        {
            var nameList = new List<string>();
            var di = new DirectoryInfo(dirABSPath);

            var dirs = di.GetDirectories();
            foreach (var d in dirs)
            {
                nameList.Add(d.Name);
            }

            return nameList;
        }

        public static string GetFileName(string absOrAssetsPath)
        {
            var name = absOrAssetsPath.Replace("\\", "/");
            var lastIndex = name.LastIndexOf("/");

            if (lastIndex >= 0)
            {
                return name.Substring(lastIndex + 1);
            }
            else
            {
                return name;
            }
        }

        public static string GetFileNameWithoutExtend(string absOrAssetsPath)
        {
            var fileName = GetFileName(absOrAssetsPath);
            var lastIndex = fileName.LastIndexOf(".");

            if (lastIndex >= 0)
            {
                return fileName.Substring(0, lastIndex);
            }
            else
            {
                return fileName;
            }
        }

        public static string GetFileExtendName(string absOrAssetsPath)
        {
            var lastIndex = absOrAssetsPath.LastIndexOf(".");

            if (lastIndex >= 0)
            {
                return absOrAssetsPath.Substring(lastIndex);
            }

            return string.Empty;
        }

        public static string GetDirPath(string absOrAssetsPath)
        {
            var name = absOrAssetsPath.Replace("\\", "/");
            var lastIndex = name.LastIndexOf("/");
            return name.Substring(0, lastIndex + 1);
        }
    }

}