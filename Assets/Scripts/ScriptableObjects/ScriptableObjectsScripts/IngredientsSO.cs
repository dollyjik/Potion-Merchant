using UnityEngine;

[CreateAssetMenu(fileName = "IngredientsSO", menuName = "Scriptable Objects/IngredientsSO")]
public class IngredientsSO : ScriptableObject
{
    [Header("Properties")]
    public string ingredientName;
    public IngredientType ingredientType;
    public Sprite ingredientIcon;
    
    [Header("Bool's")]
    public bool isFarmable; //Is it obtainable via Farming
    public bool isPurchasable; //Is it obtainable via Purchase
    
    [Header("Other Variables")]
    public int ingredientPrice; //If it is purchasable then what is it pricing on Merchant
}
