using UnityEngine;

public abstract class PlayerState : MonoBehaviour
{
    public bool IsComplete { get; protected set; }

    protected float StartTime;
    
    public float GameTime => UnityEngine.Time.time - StartTime;

    protected Rigidbody Rigidbody;
    protected CharacterController Controller;

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

    public void Setup(Rigidbody _rigidbody, CharacterController _controller)
    {
        Rigidbody = _rigidbody;
        Controller = _controller;
    }

    public void Initialise()
    {
        IsComplete = false;
        StartTime = Time.time;
    }
}
