using System;
using UnityEngine;

public class IngredientSOHolder : MonoBehaviour
{
    public IngredientsSO ingredientSO;

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("OnCollisionEnter" + other.gameObject.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter:" + other.gameObject.name);
    }
}
