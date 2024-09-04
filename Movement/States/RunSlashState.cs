using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunSlashState : MovementBaseState
{
    private bool hasPlayedAnimation = false;
    public int damage = 10;

    public override void EnterState(MovementStateManager movement)
    {
        movement.animator.SetBool("RunSlash", true);
        hasPlayedAnimation = false;
        Debug.Log("Entered RunSlash State");
    }

    public override void UpdateState(MovementStateManager movement)
    {
        AnimatorStateInfo stateInfo = movement.animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("RunSlash"))
        {
            if (stateInfo.normalizedTime >= 0.9f && !hasPlayedAnimation)
            {
                hasPlayedAnimation = true;
                movement.animator.SetBool("RunSlash", false);
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
        movement.animator.SetBool("RunSlash", false);
        Debug.Log("Exiting RunSlash State to " + state.GetType().Name);
    }

    private void OnTriggerEnter(Collider other, MovementStateManager movement)
    {
        if (other.CompareTag("Panda"))
        {
            Enemy_AI enemyAI = other.GetComponent<Enemy_AI>();
            if (enemyAI != null)
            {
                enemyAI.TakeDamage(damage); // Apply damage
                movement.StartMyCoroutine(FlashRed(enemyAI));
            }
        }
    }

    private IEnumerator FlashRed(Enemy_AI enemyAI)
    {
        Renderer renderer = enemyAI.GetComponent<Renderer>();
        Color originalColor = renderer.material.color;
        renderer.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        renderer.material.color = originalColor;
    }
}
