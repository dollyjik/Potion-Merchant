using UnityEngine;

public class FruitState : PlantBaseState
{
    public GameEvent onFruitStateFinished;
    
    public override void EnterState(PlantStateMachine stateMachine)
    {
        this.gameObject.SetActive(true);
    }

    public override void UpdateState(PlantStateMachine stateMachine)
    {
        
    }

    public override void ExitState(PlantStateMachine stateMachine)
    {
        onFruitStateFinished.Raise(this, stateMachine.currentState);

        this.gameObject.SetActive(false);
    }
}
