using UnityEngine;

public class PlantStateMachine : MonoBehaviour
{
    [Header("State References")]
    public SaplingState saplingState;
    public GrowingState growingState;
    public FruitState fruitState;
    public GrownState grownState;
    public PlantBaseState currentState;
    
    public void Start()
    {
        if (currentState == null)
        {
            currentState = saplingState;
            currentState.EnterState(this);
        }
    }

    public void Update()
    {
           currentState.UpdateState(this);
    }
    
    public void ChangeState(PlantBaseState newState)
    {
        currentState.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }

}
