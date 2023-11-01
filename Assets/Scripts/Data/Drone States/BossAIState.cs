using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossAIState : MonoBehaviour
{
    public abstract void EnterState(BossStateMachine stateMachine);
    public abstract void UpdateState(BossStateMachine stateMachine);
    public abstract void ExitState(BossStateMachine stateMachine);
}
