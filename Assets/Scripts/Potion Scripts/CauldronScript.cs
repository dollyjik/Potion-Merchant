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

    [Header("Cauldron Color")]
    [SerializeField] private Renderer cauldronRenderer;
    [SerializeField] private string shaderColorProperty = "_BaseColor";

    private void Awake()
    {
        NextRecipe();
    }

    public void NextRecipe()
    {
        if (craftingRecipeSOList == null || craftingRecipeSOList.Count == 0)
        {
            Debug.LogWarning("No crafting recipes available!");
            return;
        }

        if (craftingRecipeListIndex > craftingRecipeSOList.Count - 1)
        {
            craftingRecipeListIndex = 0;
        }
        else
        {
            craftingRecipeListIndex = (craftingRecipeListIndex + 1) % craftingRecipeSOList.Count;
            _craftingRecipeSO = craftingRecipeSOList[craftingRecipeListIndex];
        }

        UpdateRecipeDisplay();
    }

    public void PreviousRecipe()
    {
        if (craftingRecipeSOList == null || craftingRecipeSOList.Count == 0)
        {
            Debug.LogWarning("No crafting recipes available!");
            return;
        }
        if (craftingRecipeListIndex < 0)
        {
            craftingRecipeListIndex = craftingRecipeSOList.Count - 1;
        }
        else
        {
            craftingRecipeListIndex = (craftingRecipeListIndex - 1 + craftingRecipeSOList.Count) % craftingRecipeSOList.Count;
            _craftingRecipeSO = craftingRecipeSOList[craftingRecipeListIndex];
        }

        UpdateRecipeDisplay();
    }

    private void UpdateRecipeDisplay()
    {
        if (_craftingRecipeSO == null) return;

        recipeNameText.text = _craftingRecipeSO.name;

        int ingredientCount = _craftingRecipeSO.IngredientsSOList.Count;

        craftingRecipeImage1Parent.gameObject.SetActive(false);
        craftingRecipeImage2Parent.gameObject.SetActive(false);
        craftingRecipeImage3Parent.gameObject.SetActive(false);

        if (ingredientCount >= 1)
        {
            craftingRecipeImage1Parent.gameObject.SetActive(true);
            craftingRecipeImage1.sprite = _craftingRecipeSO.IngredientsSOList[0].ingredientIcon;
        }

        if (ingredientCount >= 2)
        {
            craftingRecipeImage2Parent.gameObject.SetActive(true);
            craftingRecipeImage2.sprite = _craftingRecipeSO.IngredientsSOList[1].ingredientIcon;
        }

        if (ingredientCount >= 3)
        {
            craftingRecipeImage3Parent.gameObject.SetActive(true);
            craftingRecipeImage3.sprite = _craftingRecipeSO.IngredientsSOList[2].ingredientIcon;
        }
    }

    public void Craft()
    {
        Collider[] colliderArray = Physics.OverlapBox(
            transform.position + placeItemsAreaBoxCollider.center,
            placeItemsAreaBoxCollider.size * 0.5f,
            placeItemsAreaBoxCollider.transform.rotation);

        List<IngredientsSO> inputItemList = new List<IngredientsSO>(_craftingRecipeSO.IngredientsSOList);
        List<GameObject> consumeItemGameObjectList = new List<GameObject>();

        List<Color> ingredientColors = new List<Color>();

        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent(out IngredientSOHolder ingredientSOHolder))
            {
                if (inputItemList.Contains(ingredientSOHolder.ingredientSO))
                {
                    inputItemList.Remove(ingredientSOHolder.ingredientSO);
                    consumeItemGameObjectList.Add(collider.gameObject);
                    ingredientColors.Add(ingredientSOHolder.ingredientSO.ingredientColor);
                }
            }
        }

        if (inputItemList.Count == 0)
        {
            Instantiate(_craftingRecipeSO.OutputPotionSO.PotionPrefab, itemSpawnPoint.position, itemSpawnPoint.rotation);

            foreach (GameObject consumeItemGameObject in consumeItemGameObjectList)
            {
                Destroy(consumeItemGameObject);
            }

            SetCauldronColor(_craftingRecipeSO.resultColor);
        }
        else
        {
            if (ingredientColors.Count > 0)
            {
                Color averageColor = Color.black;
                foreach (Color col in ingredientColors)
                    averageColor += col;

                averageColor /= ingredientColors.Count;
                SetCauldronColor(averageColor);
            }
        }
    }

    private void SetCauldronColor(Color color)
    {
        if (cauldronRenderer != null)
        {
            cauldronRenderer.material.SetColor(shaderColorProperty, color);
        }
    }
}
