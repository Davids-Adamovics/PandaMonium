using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovementBaseState
{
    public abstract void EnterState(MovementStateManager movement);

    public abstract void UpdateState(MovementStateManager movement);

    // Define the ExitState method here
    public virtual void ExitState(MovementStateManager movement, MovementBaseState newState)
    {
        // This can be left empty or contain some default behavior
    }
}
