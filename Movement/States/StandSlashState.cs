using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandSlashState : MovementBaseState
{
    private bool hasPlayedAnimation = false; 

    public override void EnterState(MovementStateManager movement)
    {
        movement.animator.SetBool("StandSlash", true);
        hasPlayedAnimation = false;
        Debug.Log("Entered StandSlash State");
    }

    public override void UpdateState(MovementStateManager movement)
    {
        AnimatorStateInfo stateInfo = movement.animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("StandSlash"))
        {
            if (stateInfo.normalizedTime >= 1.0f && !hasPlayedAnimation)
            {
                hasPlayedAnimation = true;
                movement.animator.SetBool("StandSlash", false);
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
        if (movement.direction.magnitude < 0.1f)
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
        movement.animator.SetBool("StandSlash", false);
        Debug.Log("Exiting StandSlash State to " + state.GetType().Name);
    }
}
