using System;
using System.Collections.Generic;
using UnityEngine;

public class CauldronScript : MonoBehaviour
{
    [SerializeField] private List<RecipeSO> craftingRecipeSOList;
    [SerializeField] private BoxCollider placeItemsAreaBoxCollider;
    [SerializeField] private RecipeSO _craftingRecipeSO;
    [SerializeField] private Transform itemSpawnPoint;
    private void Awake()
    {
        NextRecipe();
    }

    public void NextRecipe()
    {
        if (_craftingRecipeSO == null)
        {
            _craftingRecipeSO = craftingRecipeSOList[0];
        }
        else
        {
            int index = craftingRecipeSOList.IndexOf(_craftingRecipeSO);
            index = (index + 1) % craftingRecipeSOList.Count;
            _craftingRecipeSO = craftingRecipeSOList[index];
        }
    }

    public void Craft()
    {
        Collider[] colliderArray = Physics.OverlapBox(transform.position + placeItemsAreaBoxCollider.center, 
            placeItemsAreaBoxCollider.size, 
            placeItemsAreaBoxCollider.transform.rotation);

        List<IngredientsSO> inputItemList = new List<IngredientsSO>(_craftingRecipeSO.IngredientsSOList);
        List<GameObject> consumeItemGameObjectList = new List<GameObject>();
        
        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent(out IngredientSOHolder ingredientSOHolder))
            {
                if (inputItemList.Contains(ingredientSOHolder.ingredientSO))
                {
                    inputItemList.Remove(ingredientSOHolder.ingredientSO);
                    consumeItemGameObjectList.Add(collider.gameObject);
                }
            }
        }

        if (inputItemList.Count == 0 )
        {
            Instantiate(_craftingRecipeSO.OutputPotionSO.PotionPrefab, itemSpawnPoint.position, itemSpawnPoint.rotation);

            foreach (GameObject consumeItemGameObject in consumeItemGameObjectList)
            {
                Destroy(consumeItemGameObject);
            }
        }
    }
}
