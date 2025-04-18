using UnityEngine;

[CreateAssetMenu(fileName = "PotionSO", menuName = "Scriptable Objects/Potions/PotionSO")]
public class PotionSO : ScriptableObject
{
    public int PotionID;
    public string PotionName;
    public Sprite PotionIcon;
    public GameObject PotionPrefab;
}
