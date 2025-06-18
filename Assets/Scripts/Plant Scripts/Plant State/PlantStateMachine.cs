using UnityEngine;

public class PlantStateMachine : MonoBehaviour
{
    [Header("State References")]
    public SaplingState saplingState;
    public GrowingState growingState;
    public FruitState fruitState;
    public GrownState grownState;
    public PlantBaseState currentState;
    
    public JarScript[] jars;
    public void Start()
    {
        jars = FindObjectsByType<JarScript>(0);
        
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
    
    public void AddIngredientToJar(IngredientsSO ingredient)
    {
        foreach (var jar in jars)
        {
            if (jar != null && jar.GetComponent<IngredientSOHolder>().ingredientSO == ingredient)
            {
                jar.AddIngredient();
                Debug.Log($"Added {ingredient.ingredientName} to jar.");
                break;
            }
        }
    }

    public void OnEventRaised()
    {
        Debug.Log(currentState + "is finished.");
    }

}
