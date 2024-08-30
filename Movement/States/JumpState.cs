using UnityEngine;

public class JumpState : MovementBaseState
{
    private CameraShake cameraShake;

    private bool hasJumped = false;

    public override void EnterState(MovementStateManager movement)
    {
        movement.JumpForce();
        movement.jumped = true;
        movement.animator.SetTrigger("Jump");
        hasJumped = true;
        movement.jumpExplode.Play();

        // Find and reference the CameraShake component
        cameraShake = GameObject.FindObjectOfType<CameraShake>();
        if (cameraShake == null)
        {
            Debug.LogError("CameraShake component not found!");
        }
    }

    public override void UpdateState(MovementStateManager movement)
    {
        if (hasJumped)
        {
            movement.animator.SetTrigger("Falling");
            hasJumped = false;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            movement.previousState = this;
            if (cameraShake != null)
            {
                cameraShake.start = true;
            }
            movement.dashDust.Play();
            ExitState(movement, movement.Dash);
        }

        if (movement.velocity.y <= 0) 
        {
            if (movement.hzInput == 0 && movement.vInput == 0)
                movement.SwitchState(movement.Idle);
            else if (Input.GetKey(KeyCode.LeftShift))
                movement.SwitchState(movement.Run);
            else
                movement.SwitchState(movement.Walk);
        }

    }

    void ExitState(MovementStateManager movement, MovementBaseState state)
    {
        movement.animator.SetBool("Running", false);
        movement.animator.SetBool("Walking", false);
        movement.moveSpeed = 3;
        movement.SwitchState(state);
        Debug.Log("Exiting Run State to " + state.GetType().Name);
    }
}
