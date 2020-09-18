/****************************************************************************
 * Copyright (c) 2017 liuzhenhua@putao.com
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;


public enum EffectType
{
	/// <summary>
	/// 帧动画
	/// </summary>
	Animation,
	/// <summary>
	/// 粒子特效
	/// </summary>
	Particle
}


public class PPEffectConfigInfo
{
	public PPSectionInfo SectionInfo;
	public Dictionary<int, PPAnimNodeInfo> AnimNodeInfos;

	public List<PPEffectInfo> EffectInfos;

	public PPEffectConfigInfo()
	{
		AnimNodeInfos = new Dictionary<int, PPAnimNodeInfo>();
		SectionInfo = new PPSectionInfo();
		EffectInfos=new List<PPEffectInfo>();
	}
}

public class PPEffectInfo
{
	public string PID;
	public EffectType EffectType;

	public PPEffectInfo()
	{
			
	}

	public PPEffectInfo(string pid,EffectType effectType)
	{
		PID = pid;
		EffectType = effectType;
	}

	public virtual string ToString()
	{
		return string.Format("PID:{0} EffectType:{1}",PID,EffectType);
	}

	public virtual void Parse(XmlNode xmlNode)
	{
		PID = xmlNode.Attributes["pid"].Value;
		EffectType = (EffectType) Enum.Parse(typeof(EffectType), xmlNode.Attributes["effecttype"].Value);
	}
}

public class PPAnimEffectInfo:PPEffectInfo
{
	public string AnimClipName;
	public WrapMode WarpMode;
	public float Duration;

	public override string ToString()
	{
		return base.ToString() + string.Format("AnimClipName:{0} WarpMode:{1}", AnimClipName, WarpMode);
	}

	public override void Parse(XmlNode xmlNode)
	{
		base.Parse(xmlNode);
		XmlNode animNode = xmlNode.SelectSingleNode("Animation");
		AnimClipName = animNode.Attributes["animclipname"].Value;
		WarpMode = (WrapMode) Enum.Parse(typeof(WrapMode), animNode.Attributes["warpmode"].Value);
		Duration = float.Parse(animNode.Attributes["duration"].Value);
	}
}
	
public class PPParticleEffectInfo:PPEffectInfo
{
	public string ParticleObjName;
		
	public override string ToString()
	{
		return base.ToString()+string.Format("ParticleObjName:{0}",ParticleObjName);
	}
		
	public override void Parse(XmlNode xmlNode)
	{
		base.Parse(xmlNode);
		XmlNode particleNode = xmlNode.SelectSingleNode("Particle");
		ParticleObjName = particleNode.Attributes["particleobjname"].Value;
	}
}
