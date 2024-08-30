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

        // Check if we are still in the StandSlash animation
        if (stateInfo.IsName("StandSlash"))
        {
            // Check if the animation has completed
            if (stateInfo.normalizedTime >= 1.0f && !hasPlayedAnimation)
            {
                hasPlayedAnimation = true;
                movement.animator.SetBool("StandSlash", false); // Reset immediately
                DetermineNextState(movement); // Move to the next state immediately
            }
        }
        else if (hasPlayedAnimation)
        {
            // If the animation has played, determine what to do next
            DetermineNextState(movement);
        }
    }

    private void DetermineNextState(MovementStateManager movement)
    {
        // After the StandSlash animation completes, determine the next state
        if (movement.direction.magnitude < 0.1f)
        {
            movement.SwitchState(movement.Idle); // Transition back to Idle if not moving
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            movement.SwitchState(movement.Run); // Transition to Run if shift is held
        }
        else
        {
            movement.SwitchState(movement.Walk); // Otherwise, transition to Walk
        }
    }

    public override void ExitState(MovementStateManager movement, MovementBaseState state)
    {
        movement.animator.SetBool("StandSlash", false); // Ensure the StandSlash bool is reset
        Debug.Log("Exiting StandSlash State to " + state.GetType().Name);
    }
}
