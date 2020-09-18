using System;
using System.Collections.Generic;
using UnityEngine;

public class PropertyType
{
	public string Color;//Vector4 
	public string Vector;//Vector4
	public string Float;//float
	public string Range;//float
	public string Texture;//Texture
}
	
[Serializable]
public class PropInfo
{
	public string type;
	public string name;
		
}
[Serializable]
public class PropInfoVector4 :PropInfo
{
	public Vector4 data;
}

[Serializable]
public class PropInfoFloat :PropInfo
{
	public float data;
}
	
[Serializable]
public class PropInfoTexture : PropInfo
{
	public string data;
}



[Serializable]
public class MaterialInfo
{
	public string name;
	public string shaderName;
	public List<PropInfoVector4> propsVector4;
	public List<PropInfoFloat> propsFloat;
	public List<PropInfoTexture> propsTex;

	public MaterialInfo()
	{
		propsVector4 = new List<PropInfoVector4>();
		propsFloat = new List<PropInfoFloat>();
		propsTex = new List<PropInfoTexture>();
	}

	public override string ToString()
	{
		return string.Format("name:{0} shadername:{1} vector4count:{2} floatcount:{3} texcount:{4}", name, shaderName, propsVector4.Count, propsFloat.Count, propsTex.Count);
	}
}

[Serializable]
public class MaterialInfos
{
	public List<MaterialInfo> materialInfos;
}