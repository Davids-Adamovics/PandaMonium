using UnityEngine;

public class DashState : MovementBaseState
{
    private bool hasDashed = false;

    public override void EnterState(MovementStateManager movement)
    {
        movement.DashForce();
        movement.dashed = true;
        movement.animator.SetTrigger("Dash");
        hasDashed = true;
    }

    public override void UpdateState(MovementStateManager movement)
    {
        if (hasDashed)
        {
            movement.animator.SetTrigger("Running");
            movement.moveSpeed = 5;
            hasDashed = false;
        }

        if (movement.velocity.x <= 0)
        {
            if (movement.hzInput == 0 && movement.vInput == 0)
                movement.SwitchState(movement.Idle);
            else if (Input.GetKey(KeyCode.LeftShift))
                movement.SwitchState(movement.Run);
            else
                movement.SwitchState(movement.Walk);
        }

    }
}
