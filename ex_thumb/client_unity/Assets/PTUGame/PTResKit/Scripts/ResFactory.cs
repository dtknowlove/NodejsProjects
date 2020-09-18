/****************************************************************************
 * Copyright (c) 2018 ptgame@putao.com
 ****************************************************************************/


using System;
using PTGame.Core;
using UnityEngine;

namespace PTGame.ResKit
{
    public class ResFactory
    {
        static ResFactory()
        {
            SafeObjectPool<ABRes>.Instance.MaxCacheCount = 20;
            SafeObjectPool<AssetRes>.Instance.MaxCacheCount = 40;
            SafeObjectPool<InternalRes>.Instance.MaxCacheCount = 40;
            SafeObjectPool<SingleFileRes>.Instance.MaxCacheCount = 20;
        }

        public static AbstractRes Create(ResName name)
        {
            AbstractRes res = null;
            switch (name.ResType)
            {
                case eResType.Assetbundle:
                    res = SafeObjectPool<ABRes>.Instance.Allocate();
                    break;
                case eResType.Asset:
                    res = SafeObjectPool<AssetRes>.Instance.Allocate();
                    break;
                case eResType.Scene:
                    res = SafeObjectPool<SceneRes>.Instance.Allocate();
                    break;
                case eResType.Internal:
                    res = SafeObjectPool<InternalRes>.Instance.Allocate();
                    break;
                case eResType.SingleFile:
                    res = SafeObjectPool<SingleFileRes>.Instance.Allocate();
                    break;
            }

            if (res == null)
                throw new Exception(">>>[ResKit]: Create Res Error: " + name.FullName);

            res.Init(name);
            return res;
        }

        public static void Recycle(AbstractRes res)
        {
            switch (res.ResType)
            {
                case eResType.Assetbundle:
                    SafeObjectPool<ABRes>.Instance.Recycle(res as ABRes);
                    break;

                case eResType.Asset:
                    SafeObjectPool<AssetRes>.Instance.Recycle(res as AssetRes);
                    break;

                case eResType.Scene:
                    SafeObjectPool<SceneRes>.Instance.Recycle(res as SceneRes);
                    break;

                case eResType.Internal:
                    SafeObjectPool<InternalRes>.Instance.Recycle(res as InternalRes);
                    break;

                case eResType.SingleFile:
                    SafeObjectPool<SingleFileRes>.Instance.Recycle(res as SingleFileRes);
                    break;
            }
        }
    }
}
