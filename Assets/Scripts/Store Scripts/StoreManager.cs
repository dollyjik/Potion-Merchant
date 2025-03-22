using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.Serialization;

public class StoreManager : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Transform contentPanel;
    [SerializeField] GameObject storeItemPrefab;
    
    [Header("Filter Buttons")]
    [SerializeField] private Button allButton;
    [SerializeField] private Button seedButton;
    [SerializeField] private Button cropButton;
    [SerializeField] private Button miscButton;
    
    public List<IngredientsSO> allIngredients;
    
    private List<GameObject> _currentDisplayedItems = new List<GameObject>();
    private Button _currentActiveButton;
    private Color _defaultButtonColor;
    private Color _selectedButtonColor = new Color(.7f, .7f, 1f);


    private void Start()
    {
        PopulateStore("All");
        
        _defaultButtonColor = allButton.image.color;
        
        allButton.onClick.AddListener(() => FilterStore("All", allButton));
        seedButton.onClick.AddListener(() => FilterStore("Seed", seedButton));
        cropButton.onClick.AddListener(() => FilterStore("Crop", cropButton));
        miscButton.onClick.AddListener(() => FilterStore("Miscellaneous", miscButton));
        
        UpdateButtonState(allButton);
    }

    private void PopulateStore(string filter)
    {
        ClearStore();

        List<IngredientsSO> filteredIngredients = allIngredients.Where(i => i.isPurchasable) //Show Purchasable Items
            .Where(i => filter == "All" || i.ingredientType.ToString() == filter) //Filter by type
            .OrderBy(i => i.ingredientPrice) //Sort by price
            .ToList();

        foreach (var ingredient in filteredIngredients)
        {
            GameObject newItem = Instantiate(storeItemPrefab, contentPanel);
            newItem.transform.Find("IngredientNameText").GetComponent<TMP_Text>().text = ingredient.ingredientName;
            newItem.transform.Find("IngredientPriceText").GetComponent<TMP_Text>().text = "$" + ingredient.ingredientPrice;
            newItem.transform.Find("IngredientIcon").GetComponent<Image>().sprite = ingredient.ingredientIcon;
            
            Button buyButton = newItem.transform.Find("BuyButton").GetComponent<Button>();
            buyButton.onClick.AddListener(() => BuyIngredient(ingredient));
            
            _currentDisplayedItems.Add(newItem);
        }
    }

    private void BuyIngredient(IngredientsSO ingredient)
    {
        Debug.Log($"Purchased: {ingredient.ingredientName} for ${ingredient.ingredientPrice}");
    }

    private void ClearStore()
    {
        foreach (var item in _currentDisplayedItems)
        {
            Destroy(item);
        }
        _currentDisplayedItems.Clear();
    }
    
    private void FilterStore(string filter, Button selectedButton)
    {
        PopulateStore(filter);
        UpdateButtonState(selectedButton);
    }

    private void UpdateButtonState(Button selectedButton)
    {
        if (_currentActiveButton != null)
        {
            _currentActiveButton.image.color = _defaultButtonColor;
        }
        
        selectedButton.image.color = _selectedButtonColor;
        _currentActiveButton = selectedButton;
    }
}
