using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "LightingPreset", menuName = "Scriptable Objects/Lighting/LightingPreset")]

public class LightingPreset : ScriptableObject
{
    public Gradient ambientColor;
    public Gradient lightingColor;
    public Gradient fogColor;
}
