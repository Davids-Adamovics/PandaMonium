using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchState : MovementBaseState
{
    // The offset angle to rotate the character while crouching
    private float crouchRotationOffset = 35f;

    public override void EnterState(MovementStateManager movement)
    {
        movement.animator.SetBool("Crouching", true);
        Debug.Log("Entered Crouch State");
    }

    public override void UpdateState(MovementStateManager movement)
    {
        // Rotate the character with a 30-degree offset to the right
        movement.ApplyCrouchRotationOffset(crouchRotationOffset);

        if (Input.GetKey(KeyCode.LeftShift)) ExitState(movement, movement.Run);
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (movement.direction.magnitude < 0.1f) ExitState(movement, movement.Idle);
            else ExitState(movement, movement.Walk);
        }
    }

    void ExitState(MovementStateManager movement, MovementBaseState state)
    {
        movement.animator.SetBool("Crouching", false);
        movement.SwitchState(state);
        Debug.Log("Exiting Crouch State to " + state.GetType().Name);
    }
}
