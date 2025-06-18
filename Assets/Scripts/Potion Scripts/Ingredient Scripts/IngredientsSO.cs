using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "IngredientsSO", menuName = "Scriptable Objects/Potions/IngredientsSO")]
public class IngredientsSO : ScriptableObject
{
    [Header("Properties")]
    public int ingredientID;
    public string ingredientName;
    public IngredientType ingredientType;
    public Sprite ingredientIcon;
	public GameObject ingredientPrefab;
    public Color ingredientColor;
    public SeedData seedData;
    
    [Header("Bool's")]
    public bool isFarmable; //Is it obtainable via Farming
    public bool isPurchasable; //Is it obtainable via Purchase
    
    [Header("Other Variables")]
    public int ingredientPrice; //If it is purchasable then what is it pricing on Merchant
}
