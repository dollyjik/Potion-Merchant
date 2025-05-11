using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CauldronScript : MonoBehaviour
{
    [SerializeField] private List<RecipeSO> craftingRecipeSOList;
    [SerializeField] private BoxCollider placeItemsAreaBoxCollider;
    [SerializeField] private RecipeSO _craftingRecipeSO;
    [SerializeField] private Transform itemSpawnPoint;
    [SerializeField] private TextMeshProUGUI recipeNameText;
    [SerializeField] private int craftingRecipeListIndex;
    [SerializeField] private Image craftingRecipeImage1;
    [SerializeField] private Image craftingRecipeImage2;
    [SerializeField] private Image craftingRecipeImage3;
    
    [SerializeField] private Image craftingRecipeImage1Parent;
    [SerializeField] private Image craftingRecipeImage2Parent;
    [SerializeField] private Image craftingRecipeImage3Parent;
    
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
            craftingRecipeListIndex = craftingRecipeSOList.IndexOf(_craftingRecipeSO);
            craftingRecipeListIndex = (craftingRecipeListIndex + 1) % craftingRecipeSOList.Count;
            _craftingRecipeSO = craftingRecipeSOList[craftingRecipeListIndex];
        }

        recipeNameText.text = _craftingRecipeSO.name;
        if (_craftingRecipeSO.IngredientsSOList.Count() == 2)
        {
            craftingRecipeImage1.sprite = _craftingRecipeSO.IngredientsSOList[0].ingredientIcon;
            craftingRecipeImage2.sprite = _craftingRecipeSO.IngredientsSOList[1].ingredientIcon;
            craftingRecipeImage3Parent.gameObject.SetActive(false);
        }
        else if (_craftingRecipeSO.IngredientsSOList.Count() == 3)
        {
            craftingRecipeImage3.gameObject.SetActive(true);
            craftingRecipeImage1.sprite = _craftingRecipeSO.IngredientsSOList[0].ingredientIcon;
            craftingRecipeImage2.sprite = _craftingRecipeSO.IngredientsSOList[1].ingredientIcon;
            craftingRecipeImage3.sprite = _craftingRecipeSO.IngredientsSOList[2].ingredientIcon;
        }
    }

    public void PreviousRecipe()
    {
        craftingRecipeListIndex = craftingRecipeSOList.IndexOf(_craftingRecipeSO);
        craftingRecipeListIndex = (craftingRecipeListIndex - 1 + craftingRecipeSOList.Count) % craftingRecipeSOList.Count;
        _craftingRecipeSO = craftingRecipeSOList[craftingRecipeListIndex];
        recipeNameText.text = _craftingRecipeSO.name;
        if (_craftingRecipeSO.IngredientsSOList.Count() == 2)
        {
            craftingRecipeImage1.sprite = _craftingRecipeSO.IngredientsSOList[0].ingredientIcon;
            craftingRecipeImage2.sprite = _craftingRecipeSO.IngredientsSOList[1].ingredientIcon;
            craftingRecipeImage3.gameObject.SetActive(false);
        }
        else if (_craftingRecipeSO.IngredientsSOList.Count() == 3)
        {
            craftingRecipeImage3.gameObject.SetActive(true);
            craftingRecipeImage1.sprite = _craftingRecipeSO.IngredientsSOList[0].ingredientIcon;
            craftingRecipeImage2.sprite = _craftingRecipeSO.IngredientsSOList[1].ingredientIcon;
            craftingRecipeImage3.sprite = _craftingRecipeSO.IngredientsSOList[2].ingredientIcon;
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
