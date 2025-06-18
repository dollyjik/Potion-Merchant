using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.Serialization;
using System.Collections;

public class StoreManager : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Transform contentPanel;
    [SerializeField] GameObject storeItemPrefab;
    [SerializeField] MoneyManager moneyManager;
    
    [Header("Filter Buttons")]
    [SerializeField] private Button allButton;
    [SerializeField] private Button seedButton;
    [SerializeField] private Button cropButton;
    [SerializeField] private Button miscButton;
    [SerializeField] private JarScript[] jars;
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private OwlScript owl;
    
    public List<IngredientsSO> allIngredients;
    
    [SerializeField] private List<GameObject> _currentDisplayedItems = new List<GameObject>();
    private Button _currentActiveButton;

    // Ingredient kuyruğu - owl uçarken satın alınan ingredient'lar burada bekleyecek
    private Queue<IngredientsSO> pendingIngredients = new Queue<IngredientsSO>();

    private void Start()
    {
        PopulateStore("All");
        
        allButton.onClick.AddListener(() => FilterStore("All", allButton));
        seedButton.onClick.AddListener(() => FilterStore("Seed", seedButton));
        cropButton.onClick.AddListener(() => FilterStore("Crop", cropButton));
        miscButton.onClick.AddListener(() => FilterStore("Miscellaneous", miscButton));

        jars = FindObjectsByType<JarScript>(0);
        
        UpdateButtonState(allButton);
    }

    private void Update()
    {
        if (!owl.isFlying)
        { 
            ProcessPendingIngredients();
        }
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
        if (moneyManager.currentMoney > ingredient.ingredientPrice)
        {
            owl.isFlying = true;
            moneyManager.SubtractMoney(ingredient.ingredientPrice);
            Debug.Log($"Purchased: {ingredient.ingredientName} for ${ingredient.ingredientPrice}");

            if (owl.isFlying)
            {
                // Owl uçuyor, ingredient'ı kuyruğa ekle
                pendingIngredients.Enqueue(ingredient);
                Debug.Log($"Owl is flying, {ingredient.ingredientName} added to pending queue.");
            }
        }
        else
        {
            Debug.Log("Not enough money");
        }
    }

    public void ProcessPendingIngredients()
    {
        Debug.Log($"Owl landed! Processing {pendingIngredients.Count} pending ingredients.");
        
        while (pendingIngredients.Count > 0)
        {
            IngredientsSO ingredient = pendingIngredients.Dequeue();
            if (ingredient.seedData != null)
            {
                Debug.Log($"{ingredient.ingredientName} spawned.");
                Instantiate(ingredient.ingredientPrefab, spawnPoint);
            }
            else if (ingredient.seedData == null)
            {
                AddIngredientToJar(ingredient);
            }
        }
    }

    private void AddIngredientToJar(IngredientsSO ingredient)
    {
        foreach (var jar in jars)
        {
            if (jar != null && jar.GetComponent<IngredientSOHolder>().ingredientSO == ingredient)
            {
                jar.AddIngredient();
                Debug.Log($"Added {ingredient.ingredientName} to jar.");
                break;
            }
        }
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
        }
        
        _currentActiveButton = selectedButton;
    }
}