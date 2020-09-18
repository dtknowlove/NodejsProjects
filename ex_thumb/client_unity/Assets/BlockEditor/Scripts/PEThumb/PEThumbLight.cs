using UnityEngine;

public class PEThumbLight : MonoBehaviour
{
    public string lightName;

    public string prefabName
    {
        get { return string.IsNullOrEmpty(lightName) ? "pblight" : "pblight_" + lightName; }
    }
}