using System;
using UnityEngine;

public class PlantScript : MonoBehaviour
{
    private PlantStateMachine _stateMachine;

    private void Start()
    {
        _stateMachine = GetComponent<PlantStateMachine>();
    }

    private void Update()
    {
        _stateMachine.currentState.UpdateState();
    }
}
