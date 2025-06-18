using System;
using UnityEngine;

public class TezgahScript : MonoBehaviour
{
    [SerializeField] private Customer currrentCustomer;
    
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Customer"))
        {
            Customer customer = other.gameObject.GetComponent<Customer>();
            currrentCustomer = customer;
            Debug.Log(currrentCustomer);
        }

        if (other.gameObject.CompareTag("Potion") )
        {
            PotionSOHolder potionSOHolder = other.gameObject.GetComponent<PotionSOHolder>();
            Debug.Log(potionSOHolder.potionSO);
            currrentCustomer.RecivePotion(potionSOHolder.potionSO);
            Destroy(other.gameObject);
        }
    }
}
