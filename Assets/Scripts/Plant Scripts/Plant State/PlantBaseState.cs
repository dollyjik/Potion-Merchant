using UnityEngine;

public abstract class PlantBaseState : MonoBehaviour
{
    public abstract void EnterState(PlantStateMachine stateMachine);
    
    public abstract void UpdateState(PlantStateMachine stateMachine);
    
    public abstract void ExitState(PlantStateMachine stateMachine);
}
