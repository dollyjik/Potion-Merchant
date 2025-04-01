using UnityEngine;

public class PlantStateMachine : BaseStateMachine<PlantBaseState>
{
    [Header("State References")]
    [SerializeField] private SaplingState saplingState;
    [SerializeField] private GrowingState growingState;
    [SerializeField] private FruitState fruitState;
    [SerializeField] private GrownState grownState;
    public PlantBaseState currentState;
    
    [Header("Other References")]
    [SerializeField] private DayManager dayManager;
    [SerializeField] private int plantDay;
    [SerializeField] private float plantTime;
    public override void Start()
    {
        dayManager = FindAnyObjectByType<DayManager>();
        plantDay = dayManager.currentDay;
        plantTime = dayManager.timeOfDay;
        
        if(currentState == null)
            currentState = saplingState;
    }

    public override void Update()
    {
        
    }
    
    public override void ChangeState(PlantBaseState newState)
    {
        currentState.ExitState();
        currentState = newState;
    }

}
