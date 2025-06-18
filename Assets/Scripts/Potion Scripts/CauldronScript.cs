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

    [SerializeField] private GameObject smokeEffect;
    [SerializeField] private AudioClip brewSound;
    [SerializeField] private AudioClip failSound;
    [SerializeField] private AudioClip boilSound;
    [SerializeField] private AudioClip splashSound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource owlAudioSource;
    
    [Header("Cauldron Color")]
    [SerializeField] private Renderer cauldronRenderer;
    [SerializeField] private string shaderColorProperty = "_BaseColor";
    [SerializeField] private Color defaultColor = Color.black;

    private List<IngredientsSO> currentIngredients = new List<IngredientsSO>();
    private List<Color> currentIngredientColors = new List<Color>();

    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        NextRecipe();
        audioSource.clip = boilSound;
        audioSource.volume = 0.010f;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void OnIngredientEntered(Collider other)
    {
        if (other.TryGetComponent(out IngredientSOHolder ingredientSOHolder))
        {
            audioSource.PlayOneShot(splashSound, 7f);
            IngredientsSO ingredient = ingredientSOHolder.ingredientSO;

            currentIngredients.Add(ingredient);
            currentIngredientColors.Add(ingredient.ingredientColor);

            UpdateCauldronColor();
            Destroy(other.gameObject);

            Debug.Log($"Kazanda yeni ingredient: {ingredient.name}");
        }
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
            craftingRecipeListIndex++;
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
        if (_craftingRecipeSO == null) return;

        if (currentIngredients.Count != _craftingRecipeSO.IngredientsSOList.Count)
        {
            Debug.LogWarning("Gerekli ingredient sayısı uyuşmuyor!");
            return;
        }

        List<IngredientsSO> required = new List<IngredientsSO>(_craftingRecipeSO.IngredientsSOList);
        List<IngredientsSO> given = new List<IngredientsSO>(currentIngredients);

        bool matches = required.All(ing => given.Remove(ing)) && given.Count == 0;

        if (matches)
        {
            Instantiate(_craftingRecipeSO.OutputPotionSO.PotionPrefab, itemSpawnPoint.position, itemSpawnPoint.rotation);
            Instantiate(smokeEffect, itemSpawnPoint.position, Quaternion.identity);
            audioSource.loop = false;
            audioSource.PlayOneShot(brewSound, 7);
            audioSource.loop = true;
            audioSource.volume = 0.010f;
            audioSource.clip = boilSound;
            audioSource.Play();
            Debug.Log("Craft başarılı! İksir üretildi.");
            ClearCauldron();
        }
        else
        {
            owlAudioSource.PlayOneShot(failSound, .25f);
            Debug.LogWarning("Yanlış ingredient kombinasyonu!");
        }
    }

    private void UpdateCauldronColor()
    {
        if (currentIngredientColors.Count == 0)
        {
            SetCauldronColor(defaultColor);
            return;
        }

        Color averageColor = Color.black;
        foreach (Color col in currentIngredientColors)
        {
            averageColor += col;
        }

        averageColor /= currentIngredientColors.Count;
        SetCauldronColor(averageColor);
    }

    private void SetCauldronColor(Color color)
    {
        if (cauldronRenderer != null)
        {
            cauldronRenderer.material.SetColor(shaderColorProperty, color);
        }
    }

    public void ClearCauldron()
    {
        currentIngredients.Clear();
        currentIngredientColors.Clear();
        SetCauldronColor(defaultColor);
        Debug.Log("Kazan temizlendi.");
    }

    public List<IngredientsSO> GetCurrentIngredients()
    {
        return new List<IngredientsSO>(currentIngredients);
    }
}
