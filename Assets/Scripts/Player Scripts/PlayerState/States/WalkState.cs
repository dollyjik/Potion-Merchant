using UnityEngine;

public class WalkState : PlayerState
{
    public override void Enter()
    {
        
    }

    public override void Do()
    {
        
    }

    public override void FixedDo()
    {
        PlayController.HandleMovement();
    }

    public override void Exit()
    {
        
    }
}
