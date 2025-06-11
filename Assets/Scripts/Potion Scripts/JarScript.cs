using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JarScript : MonoBehaviour
{
    private IngredientsSO jarIngredient;
    public int jarID;
    [SerializeField] private TextMeshProUGUI ingredientName;
    [SerializeField] private TextMeshProUGUI ingredientCount;
    [SerializeField] private Image ingredientImage;
    [SerializeField] private int maxIngredientCount = 20;
    [SerializeField] private int currentIngredientCount = 0;
    private void Start()
    {
        jarIngredient = GetComponent<IngredientSOHolder>().ingredientSO;
        ingredientName.text = jarIngredient.ingredientName;
        ingredientImage.sprite = jarIngredient.ingredientIcon;
        ingredientCount.text = currentIngredientCount.ToString() + "/" + maxIngredientCount.ToString();
    }


    public GameObject SpawnIngredient(Transform transform)
    {
        if (currentIngredientCount > 0)
        {
            GameObject spawned = Instantiate(jarIngredient.ingredientPrefab, transform);
            currentIngredientCount--;
            UpdateJarUI();
            return spawned;
        }

        return null;
    }
    
    public void AddIngredient()
    {
        if (currentIngredientCount >= maxIngredientCount)
        {
            Debug.Log("Jar is full! Cannot add more ingredients.");
        }

        currentIngredientCount++;
        UpdateJarUI();
    }

    private void UpdateJarUI()
    {
        ingredientCount.text = currentIngredientCount.ToString() + "/" + maxIngredientCount.ToString();
    }
}
