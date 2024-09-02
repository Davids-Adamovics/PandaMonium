using System.Collections;
using UnityEngine;

public class WalkState : MovementBaseState
{
    private CameraShake cameraShake;

    public override void EnterState(MovementStateManager movement)
    {
        movement.animator.SetBool("Walking", true);
        movement.moveSpeed = 3; // Set an appropriate walking speed
        Debug.Log("Entered Walk State");

        // Find and reference the CameraShake component
        cameraShake = GameObject.FindObjectOfType<CameraShake>();
        if (cameraShake == null)
        {
            Debug.LogError("CameraShake component not found!");
        }
    }

    public override void UpdateState(MovementStateManager movement)
    {
        movement.dust.Play();

        // RUN
        if (Input.GetKey(KeyCode.LeftShift))
        {
            ExitState(movement, movement.Run);
        }
        // CROUCH
        else if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            movement.previousState = this;
            ExitState(movement, movement.Crouch);
        }
        // SLASH + RUN
        else if (Input.GetKeyDown(KeyCode.Mouse0) && Input.GetKey(KeyCode.LeftShift))
        {
            movement.previousState = this;
            ExitState(movement, movement.RunSlash);
        }
        // RANDOM SLASH (WalkSlash or StandSlash)
        else if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            movement.previousState = this;
            if (Random.value > 0.5f) // 50% chance
            {
                ExitState(movement, movement.WalkSlash); // Transition to WalkSlash
            }
            else
            {
                ExitState(movement, movement.StandSlash); // Transition to StandSlash
            }
        }
        // JUMP
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            movement.previousState = this;
            ExitState(movement, movement.Jump);
        }
        // IDLE
        else if (movement.direction.magnitude < 0.1f)
        {
            ExitState(movement, movement.Idle);
        }
    }

    void ExitState(MovementStateManager movement, MovementBaseState state)
    {
        movement.animator.SetBool("Walking", false);
        movement.SwitchState(state);
        Debug.Log("Exiting Walk State to " + state.GetType().Name);
    }
}
