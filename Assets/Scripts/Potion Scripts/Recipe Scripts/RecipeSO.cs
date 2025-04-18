using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipeSO", menuName = "Scriptable Objects/Potions/RecipeSO")]
public class RecipeSO : ScriptableObject
{
    public int RecipeID;
    public Sprite Sprite;
    public List<IngredientsSO> IngredientsSOList;
    public PotionSO OutputPotionSO;
}
