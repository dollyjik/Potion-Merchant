using UnityEngine;

public class CauldronTriggerRelay : MonoBehaviour
{
    [SerializeField] private CauldronScript cauldron;

    private void OnTriggerEnter(Collider other)
    {
        cauldron.OnIngredientEntered(other);
    }
}
