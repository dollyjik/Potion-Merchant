using UnityEngine;

public class GrowingState : PlantBaseState
{
    public float growDuration;
    private float _timeElapsed;
    
    public Vector3 startScale;
    public Vector3 finishScale;

    public GameEvent onGrowingStateFinished;
    
    public override void EnterState(PlantStateMachine stateMachine)
    {
        this.gameObject.SetActive(true);
    }

    public override void UpdateState(PlantStateMachine stateMachine)
    {
        if (_timeElapsed < growDuration)
        {
            _timeElapsed += Time.deltaTime;
            float t = _timeElapsed / (growDuration);
            transform.localScale = Vector3.Lerp(startScale, finishScale, t);
        }
        else
        {
            stateMachine.ChangeState(stateMachine.fruitState);
        }
    }

    public override void ExitState(PlantStateMachine stateMachine)
    {
        onGrowingStateFinished.Raise(this, stateMachine.currentState);

        this.gameObject.SetActive(false);
    }
}