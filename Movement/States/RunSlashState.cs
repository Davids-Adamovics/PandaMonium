using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunSlashState : MovementBaseState
{
    private bool hasPlayedAnimation = false;

    public override void EnterState(MovementStateManager movement)
    {
        movement.animator.SetBool("RunSlash", true);
        hasPlayedAnimation = false;
        Debug.Log("Entered RunSlash State");
    }

    public override void UpdateState(MovementStateManager movement)
    {
        AnimatorStateInfo stateInfo = movement.animator.GetCurrentAnimatorStateInfo(0);

        // Check if the RunSlash animation is active
        if (stateInfo.IsName("RunSlash"))
        {
            // If the animation is nearly complete, prepare to transition
            if (stateInfo.normalizedTime >= 0.9f && !hasPlayedAnimation)
            {
                hasPlayedAnimation = true;
                movement.animator.SetBool("RunSlash", false);
                DetermineNextState(movement); // Transition to the next state immediately
            }
        }
        else if (hasPlayedAnimation)
        {
            DetermineNextState(movement);
        }
    }

    private void DetermineNextState(MovementStateManager movement)
    {
        // Determine the next state based on input
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
        movement.animator.SetBool("RunSlash", false); // Reset the RunSlash trigger
        Debug.Log("Exiting RunSlash State to " + state.GetType().Name);
    }
}
