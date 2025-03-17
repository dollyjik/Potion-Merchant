using UnityEngine;

public class JumpState : PlayerState
{
    public override void Enter()
    {
        
    }

    public override void Do()
    {
        Controller.Jump();
        Controller.readyToJump = false;
        
        if (Controller.isGrounded)
        {
            IsComplete = true;
            Controller.readyToJump = true;
        }   
    }

    public override void FixedDo()
    {
        
    }

    public override void Exit()
    {
        
    }
}
