using System;
using UnityEngine;

public class PotionScript : MonoBehaviour
{
    [SerializeField] private Renderer potionRenderer;
    private PotionSOHolder potionSOHolder;

    private void Start()
    {
        potionSOHolder = GetComponent<PotionSOHolder>();
        potionRenderer  = GetComponent<Renderer>();

        Material instanceMaterial = potionRenderer.material;
        instanceMaterial.color = potionSOHolder.potionSO.PotionColor;
    }
}