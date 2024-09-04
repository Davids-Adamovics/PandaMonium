using System.Collections;
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
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            movement.previousState = this;
            if (Random.value > 0.5f) 
            {
                ExitState(movement, movement.StandSlash); 
            }
            else
            {
                ExitState(movement, movement.WalkSlash);
            }
            return;
        }

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
