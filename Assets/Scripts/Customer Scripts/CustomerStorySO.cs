using UnityEngine;

[CreateAssetMenu(fileName = "CustomerStorySO", menuName = "Scriptable Objects/Customer/CustomerStorySO")]
public class CustomerStorySO : ScriptableObject
{
    [TextArea(2, 5)] public string[] storyLines;
    public PotionSO wantedPotion;
}
