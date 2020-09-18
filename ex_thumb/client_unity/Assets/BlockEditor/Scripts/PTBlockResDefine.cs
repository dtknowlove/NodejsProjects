using System;
using System.Collections.Generic;
using UnityEngine.Events;

public enum ResType
{
    Block_Thumbs,
    Block_Fbxs,
    Block_Prefabs,
    Block_Textures,
    Texture,
    Shader
}

[Serializable]
public class ResConfigItem
{
    public int index = 0;
    public string name ="";
    public string md5 = "";
    public string size = "";
}

[Serializable]
public class ResConfig
{
    public List<ResConfigItem> Items = new List<ResConfigItem>();
}

public class ResUpdateInfo
{
    public int CategoryId;
    public string[] PrefabList;
    public string[] ThumbList;
}

public enum DownloadState
{
    None,
    Error,
    
    ResConfig,
    ResConfigFinish,
    
    ABManifest,
    ABManifestFinish,
    
    Prefabs,
    PrefabsFinish,
    
    Thumbs,
    ThumbsFinish,
    
    Finish,
}

/// <summary>
/// message to be carried when firing download state event
/// </summary>
public class ResUpdateMsg
{
    public DownloadState State;
    public string MsgText;
    public float Progress;
}

public class ResUpdateEvent : UnityEvent<ResUpdateMsg> {}