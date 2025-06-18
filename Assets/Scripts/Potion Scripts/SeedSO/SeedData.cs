using UnityEngine;

[CreateAssetMenu(fileName = "SeedSO", menuName = "Scriptable Objects/Ingredients/SeedSO")]
public class SeedData : ScriptableObject
{
    [Header("Seed Information")]
    public string seedName;
    public Sprite seedIcon; // Envanterde göstermek için

    [Header("Plant Information")]
    public GameObject plantPrefab; // Bu tohum ekildiğinde oluşacak bitkinin prefab'ı
}