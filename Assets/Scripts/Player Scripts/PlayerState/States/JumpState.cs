using UnityEngine;

public class JumpState : PlayerState
{
    public override void Enter()
    {
        
    }

    public override void Do()
    {
        PlayController.Jump();
        
        if (PlayController.isGrounded)
        {
            IsComplete = true;
        }   
    }

    public override void FixedDo()
    {
        
    }

    public override void Exit()
    {
        
    }
}
