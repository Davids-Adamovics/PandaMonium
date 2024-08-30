using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        movement.animator.SetBool("Running", false);
        movement.animator.SetBool("Walking", false);
        movement.moveSpeed = 0;
        Debug.Log("Entered Idle State");
    }

    public override void UpdateState(MovementStateManager movement)
    {
        // Start slash immediately when mouse button is clicked
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            movement.previousState = this;
            ExitState(movement, movement.StandSlash);
            return; // Immediately return to ensure no other transitions happen in the same frame
        }

        // Check movement to switch to running or walking
        if (movement.direction.magnitude > 0.1f)
        {
            if (Input.GetKey(KeyCode.LeftShift))
                movement.SwitchState(movement.Run);
            else
                movement.SwitchState(movement.Walk);
        }
        // CROUCH
        else if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            movement.previousState = this;
            ExitState(movement, movement.Crouch);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            movement.previousState = this;
            movement.SwitchState(movement.Jump);
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            movement.previousState = this;
            movement.SwitchState(movement.Dash);
        }
    }

    void ExitState(MovementStateManager movement, MovementBaseState state)
    {
        movement.SwitchState(state);
        Debug.Log("Exiting Idle State to " + state.GetType().Name);
    }
}
