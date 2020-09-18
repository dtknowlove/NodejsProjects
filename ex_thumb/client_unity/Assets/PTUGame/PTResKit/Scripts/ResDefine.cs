/****************************************************************************
 * Copyright (c) 2018 ptgame@putao.com
 ****************************************************************************/


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace PTGame.ResKit
{
    public enum eResState
    {
        Waiting = 0,
        Loading,
        Ready,
    }

    public enum eResType
    {
        Assetbundle = 0,
        Asset,
        Scene,
        Internal,    //Resources.Load()
        SingleFile,    //www.load text/texture/bytes files
    }

    [Serializable]
    public class ABData
    {
        public string name;
        public string[] assets;
    }
    

    [Serializable]
    public class ABDataList
    {
        public List<ABData> abInfos = new List<ABData>();
    }
    
    
    public class ResLoadEvent : UnityEvent<bool, AbstractRes>{}

    public class ResTask
    {
        private readonly IEnumerator mTask;

        public ResTask(IEnumerator task)
        {
            mTask = task;
        }

        public IEnumerator Start(Action onFinish)
        {
            yield return mTask;
            if (onFinish != null)
                onFinish();
        }
    }
    
    
    public class SearchResNameFailInfo
    {
        public const int NOTFOUND_AB = 1;
        public const int NOTFOUND_ASSET = 2;
        public const int NAME_AMBIGUOUS = 3;

        public readonly int FailType;

        public SearchResNameFailInfo(int failType)
        {
            this.FailType = failType;
        }

        public override string ToString()
        {
            switch (FailType)
            {
                case NOTFOUND_AB:
                    return ">>>[ResKit]: assetbundle \"{0}\" is not found. Please check if you have build it";
                case NOTFOUND_ASSET:
                    return ">>>[ResKit]: asset name \"{0}\" is not found. Please check if you have build it into the assetbundle.";
                case NAME_AMBIGUOUS:
                    return ">>>[ResKit]: multiple assets share the same name \"{0}\". Please provide assetName with extension, or assetbundle name, or both!";

            }
            return null;
        }
    }

    [Serializable]
    public struct LoadedABInfo
    {
        public string abName;
        public string abGroup;
        public int refCount;

        public LoadedABInfo(string abGroup, string abName, int refCount)
        {
            this.abGroup = abGroup;
            this.abName = abName;
            this.refCount = refCount;
        }
    }
}