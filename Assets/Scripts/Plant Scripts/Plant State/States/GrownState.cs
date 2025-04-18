using UnityEngine;

public class GrownState : PlantBaseState
{
    public GameEvent onGrownStateFinished;
    public override void EnterState(PlantStateMachine stateMachine)
    {
        this.gameObject.SetActive(true);
    }

    public override void UpdateState(PlantStateMachine stateMachine)
    {
        
    }

    public override void ExitState(PlantStateMachine stateMachine)
    {
        onGrownStateFinished.Raise(this, stateMachine.currentState);
        this.gameObject.SetActive(false);

    }
}