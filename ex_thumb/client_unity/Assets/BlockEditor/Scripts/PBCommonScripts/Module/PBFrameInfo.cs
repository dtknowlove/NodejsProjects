/****************************************************************************
 * Copyright (c) 2017 liuzhenhua@putao.com
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Putao.PaiBloks.Common
{
    /// <summary>
    /// 零件信息
    /// </summary>
    public class PBPartInfo
    {
        /// <summary>
        /// 缩略图名称，唯一标识
        /// </summary>
        public string ThumbName
        {
            get { return BlockInfo.Thumb; }
        }

        /// <summary>
        /// Block prefab name
        /// </summary>
        public string PrefabName
        {
            get { return BlockInfo.Prefab; }
        }

        public int Count;

        public PPBlockInfo BlockInfo { get; private set; }
        public PBPartDetail PartDetail { get; private set; }

        public PBPartInfo(PPBlockInfo blockInfo, int count)
        {
            BlockInfo = blockInfo;
            Count = count;
            PartDetail = PBPartDetail.Parse(blockInfo.Detail);
        }

        public string ToString()
        {
            return string.Format("Name:{0}, Thumb: {1}, Count:{2}", PrefabName, ThumbName, Count);
        }
    }

    /// <summary>
    /// 零件展示信息
    /// </summary>
    [Serializable]
    public class PBPartDetail
    {
        public List<PBDetailInfo> Infos;
        
        public string GetValue(PBDetailType dType)
        {
            if (Infos == null)
                return null;
            
            var info = Infos.Find(t => t.Type == dType);
            if (info != null)
                return info.Value;
            return null;
        }

        public void SetValue(PBDetailType dType, string value)
        {
            var info = Infos.Find(t => t.Type == dType);
            if (info != null)
                info.Value = value;
            else
                Infos.Add(new PBDetailInfo(dType, value));
        }

        /// <summary>
        /// 零件描述
        /// </summary>
        public string Description
        {
            get { return GetValue(PBDetailType.Desc); }
        }

        /// <summary>
        /// 编号
        /// </summary>
        public string Number
        {
            get { return GetValue(PBDetailType.Numb); }
        }

        public PBPartDetail(string info)
        {
            Infos = new List<PBDetailInfo>();
            if (!string.IsNullOrEmpty(info))
            {
                var splitInfos = info.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                foreach (var s in splitInfos)
                {
                    Infos.Add(PBDetailInfo.Parse(s));
                }    
            }
        }

        public static PBPartDetail Parse(string info)
        {
            return new PBPartDetail(info);
        }

        public override string ToString()
        {
            var result = string.Empty;
            if (Infos == null)
                return result;
            foreach (var info in Infos)
            {
                if (info == null)
                    continue;
                if (!string.IsNullOrEmpty(info.Value))
                    result = result + (info + ",");
            }
            return result;
        }
    }

    public enum PBPointType
    {
        None = 0,
        SectionPoint = 1, //节点
        GuidePoint = 2, //特殊说明
    }

    /// <summary>
    /// 搭建节点信息
    /// </summary>
    public class PBPointInfo
    {
        /// <summary>
        /// 第几步
        /// </summary>
        public int StepIndex;

        /// <summary>
        /// 类型
        /// </summary>
        public PBPointType PointType;

        /// <summary>
        /// 动画信息
        /// </summary>
        public string AnimInfo;

        /// <summary>
        /// 提示信息
        /// </summary>
        public string TipInfo;

        /// <summary>
        /// 是否有提示信息
        /// </summary>
        public bool HasTipInfo
        {
            get { return !string.IsNullOrEmpty(TipInfo); }
        }

        public PBPointInfo(int stepIndex, PBPointType type, string animInfo, string tipInfo)
        {
            StepIndex = stepIndex;
            PointType = type;
            AnimInfo = animInfo;
            TipInfo = tipInfo;
        }

        private PBPointIntro mSpriteAnimInfo = null;

        /// <summary>
        /// 单独一个 鼓励动画节点用
        /// </summary>
        /// <returns></returns>
        public PBPointIntro SpriteAnimInfo
        {
            get
            {
                if (mSpriteAnimInfo == null)
                    mSpriteAnimInfo = GetPointIntroList()[0];
                return mSpriteAnimInfo;
            }
        }

        public List<PBPointIntro> GetPointIntroList()
        {
            //兼容老版本
            if (AnimInfo.StartsWith("tire"))
            {
                return new List<PBPointIntro>()
                {
                    new PBPointIntro(ContentType.Tire, "tire", "", "", "AskHelp")
                };
            }
            var animList = AnimInfo.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
            return animList.Select(PBPointIntro.Parse).ToList();
        }

        public override string ToString()
        {
            return string.Format("StepIndex:{0}, PointType: {1}, AnimInfo: {2}, TipInfo: {3}", StepIndex, PointType, AnimInfo, TipInfo);
        }
    }

    public enum ContentType
    {
        Tire = 0, //老版本轮胎动画
        Anim, //动画
        Img, //图片
        Video, //视频
    }

    /// <summary>
    /// 搭建节点的展示信息
    /// </summary>
    [Serializable]
    public class PBPointIntro
    {
        public ContentType Type;
        public string AssetName;
        public string AnimName;
        public string AudioName;
        public string TipContent;

        public PBPointIntro(ContentType type, string assetName, string animName = "", string audioName = "",
            string tipContent = "")
        {
            Type = type;
            AssetName = assetName;
            AnimName = animName;
            AudioName = audioName;
            TipContent = tipContent;
        }

        public static PBPointIntro Parse(string animInfo)
        {
            ContentType type = (ContentType) Enum.Parse(typeof(ContentType), animInfo.Split('_')[0]);
            var assetName = string.Empty;
            var animName = string.Empty;
            var audioName = string.Empty;
            var tipContent = string.Empty;
            var itItems = animInfo.Split(',');
            foreach (var t in itItems)
            {
                var splits = t.Split('_');
                if (splits[0].StartsWith("Audio"))
                {
                    audioName = splits[1];
                }
                else if (splits[0].StartsWith("Tip"))
                {
                    tipContent = splits[1];
                }
                else
                {
                    assetName = splits[1];
                    if (splits.Length >= 3)
                        animName = splits[2];
                }
            }
            return new PBPointIntro(type, assetName, animName, audioName, tipContent);
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(AssetName))
                return "";

            var result = string.Format("{0}_{1}", Type, AssetName);
            if (!string.IsNullOrEmpty(AnimName))
                result += "_" + AnimName;
            if (!string.IsNullOrEmpty(AudioName))
                result += ",Audio_" + AudioName;
            if (!string.IsNullOrEmpty(TipContent))
                result += ",Tip_" + TipContent;
            return result;
        }
    }

    /// <summary>
    /// 零件类型信息
    /// </summary>
    public class PartTypeInfo
    {
        /// <summary>
        /// 颗粒背景色
        /// </summary>
        public Color BgColor { get; private set; }

        /// <summary>
        /// 角标颜色
        /// </summary>
        public Color CornerColor { get; private set; }

        /// <summary>
        /// 角标文字
        /// </summary>
        public string CornerText { get; private set; }

        /// <summary>
        /// 提示文字
        /// </summary>
        public string TipText { get; private set; }

        public PartTypeInfo()
        {
        }

        public PartTypeInfo(Color bgColor, Color cornerColor, string cornerText, string tipText)
        {
            BgColor = bgColor;
            CornerColor = cornerColor;
            CornerText = cornerText;
            TipText = tipText;
        }

        public PartTypeInfo SetBgColor(Color c)
        {
            BgColor = c;
            return this;
        }

        public PartTypeInfo SetCornerColor(Color c)
        {
            CornerColor = c;
            return this;
        }

        public PartTypeInfo SetCornerText(string t)
        {
            CornerText = t;
            return this;
        }

        public PartTypeInfo SetTipText(string t)
        {
            TipText = t;
            return this;
        }
    }

    /// <summary>
    /// 所有以字符串标识的类型后续都会添加进来
    /// </summary>
    public enum PBDetailType
    {
        Desc = 0, //描述
        Numb //编号
    }

    [Serializable]
    public class PBDetailInfo
    {
        public PBDetailType Type;
        public string Value;

        public PBDetailInfo(string type, string value)
        {
            Type = (PBDetailType) Enum.Parse(typeof(PBDetailType), type);
            Value = value;
        }

        public PBDetailInfo(PBDetailType type, string value)
        {
            Type = type;
            Value = value;
        }

        public static PBDetailInfo Parse(string info)
        {
            var splitInfos = info.Split('_');
            if (splitInfos.Length < 2)
            {
                Debug.LogErrorFormat("The Config Info is Error! Error info:{0}", info);
                return null;
            }
            if (splitInfos.Length > 2)
            {
                Debug.LogErrorFormat("The Config Info'value is not allowed \"_\"! Error info:{0}", info);
                return null;
            }
            return new PBDetailInfo(splitInfos[0], splitInfos[1]);
        }

        public override string ToString()
        {
            return string.Format("{0}_{1}", Type, Value);
        }
    }
}