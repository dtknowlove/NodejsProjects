using System.IO;
using System.Xml;
using UnityEngine;

public class PBLightingLiteracy
{
    public static string TempSavePath()
    {
        string directory = Path.Combine(Path.GetDirectoryName(Application.dataPath), "SaveData");
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        return Path.Combine(directory, "light_applied.txt");
    }

    public static void SaveLocal(string lightName)
    {
        File.WriteAllText(TempSavePath(), lightName);
    }

    public static string ReadLocal()
    {
        string savePath = TempSavePath();
        return File.Exists(savePath) ? File.ReadAllText(savePath) : PBLighting.DEFAULT;
    }

    #region write

    public static void SaveLightingConfig(XmlDocument xml, XmlElement parent)
    {
        string text = ReadLocal();
        
        XmlElement keyElement = xml.CreateElement("lighting");
        keyElement.SetAttribute("data", text);
        parent.AppendChild(keyElement);
    }
    
    #endregion

    #region read

    public static string LoadLightingConfig(XmlDocument xml)
    {
        string lightName = PBLighting.DEFAULT;
        
        XmlNode node = xml.SelectSingleNode("config").SelectSingleNode("lighting");
        if (node != null && node.Attributes["data"] != null)
            lightName = node.Attributes["data"].Value;
        
        SaveLocal(lightName);
        return lightName;
    }

    #endregion
}