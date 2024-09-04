using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkSlashState : MovementBaseState
{
    private bool hasPlayedAnimation = false;

    public override void EnterState(MovementStateManager movement)
    {
        movement.animator.SetBool("WalkSlash", true);
        hasPlayedAnimation = false;
        Debug.Log("Entered WalkSlash State");
    }

    public override void UpdateState(MovementStateManager movement)
    {
        AnimatorStateInfo stateInfo = movement.animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("WalkSlash"))
        {
            if (stateInfo.normalizedTime >= 0.9f && !hasPlayedAnimation)
            {
                hasPlayedAnimation = true;
                movement.animator.SetBool("WalkSlash", false);
                DetermineNextState(movement);
            }
        }
        else if (hasPlayedAnimation)
        {
            DetermineNextState(movement);
        }
    }


    private void DetermineNextState(MovementStateManager movement)
    {
        if (movement.hzInput == 0 && movement.vInput == 0)
        {
            movement.SwitchState(movement.Idle);
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            movement.SwitchState(movement.Run);
        }
        else
        {
            movement.SwitchState(movement.Walk);
        }
    }

    public override void ExitState(MovementStateManager movement, MovementBaseState state)
    {
        movement.animator.SetBool("WalkSlash", false);
        Debug.Log("Exiting WalkSlash State to " + state.GetType().Name);
    }
}
