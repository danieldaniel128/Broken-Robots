using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChipAIState
{
    public abstract void EnterState(ChipStateMachine stateMachine);
    public abstract void UpdateState(ChipStateMachine stateMachine);
    public abstract void ExitState(ChipStateMachine stateMachine);
}

