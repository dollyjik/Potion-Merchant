using System;
using UnityEngine;

public class PlantScript : MonoBehaviour
{
    
    [Header("Other References")]
    [SerializeField] private DayManager dayManager;
    [SerializeField] private int plantDay;
    [SerializeField] private float plantTime;
    
    [SerializeField] private PlantStateMachine stateMachine;

    private void Start()
    {
        stateMachine = GetComponent<PlantStateMachine>();
        
        dayManager = FindAnyObjectByType<DayManager>();
        plantDay = dayManager.currentDay;
        plantTime = dayManager.timeOfDay;
    }

    private void Update()
    {
    }
}
