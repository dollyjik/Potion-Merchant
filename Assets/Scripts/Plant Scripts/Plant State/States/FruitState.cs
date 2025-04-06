using UnityEngine;

public class FruitState : PlantBaseState
{
    public override void EnterState(PlantStateMachine stateMachine)
    {
        this.gameObject.SetActive(true);
    }

    public override void UpdateState(PlantStateMachine stateMachine)
    {
        
    }

    public override void ExitState(PlantStateMachine stateMachine)
    {
        this.gameObject.SetActive(false);
    }
}
