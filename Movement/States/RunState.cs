using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunState : MovementBaseState
{
    private float runRotationOffset = 35f; 

    public override void EnterState(MovementStateManager movement)
    {
        movement.animator.SetBool("Running", true);
        movement.animator.SetBool("Walking", true);
        movement.moveSpeed = 7;
        Debug.Log("Entered Run State");
    }

    public override void UpdateState(MovementStateManager movement)
    {
        movement.dust.Play();

        if (Input.GetKeyDown(KeyCode.Mouse0) && Input.GetKey(KeyCode.LeftShift))
        {
            movement.previousState = this;
            ExitState(movement, movement.RunSlash);
            return; 
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            ExitState(movement, movement.Walk);
        }
        else if (movement.direction.magnitude < 0.1f)
        {
            ExitState(movement, movement.Idle);
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0) && !Input.GetKey(KeyCode.LeftShift) && movement.direction.magnitude >= 0.1f)
        {
            movement.previousState = this;
            ExitState(movement, movement.WalkSlash);
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0) && movement.direction.magnitude < 0.1f)
        {
            movement.previousState = this;
            ExitState(movement, movement.StandSlash);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            movement.previousState = this;
            ExitState(movement, movement.Jump);
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            movement.previousState = this;
            ExitState(movement, movement.Dash);
        }
    }

    void ExitState(MovementStateManager movement, MovementBaseState state)
    {
        movement.animator.SetBool("Running", false);
        movement.animator.SetBool("Walking", false);
        movement.moveSpeed = 5;
        movement.SwitchState(state);
        Debug.Log("Exiting Run State to " + state.GetType().Name);
    }
}
