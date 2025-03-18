using UnityEngine;

public abstract class PlayerState : MonoBehaviour
{
    public bool IsComplete { get; protected set; }

    protected float StartTime;
    
    public float GameTime => UnityEngine.Time.time - StartTime;

    protected CharacterController CharController;
    protected PlayerController PlayController;

    public virtual void Enter()
    {
        
    }

    public virtual void Do()
    {
        
    }

    public virtual void FixedDo()
    {
        
    }
    
    public virtual void Exit()
    {
        
    }

    public void Setup(CharacterController charController, PlayerController playController)
    {
        CharController = charController;
        PlayController = playController;
    }

    public void Initialise()
    {
        IsComplete = false;
        StartTime = Time.time;
    }
}
