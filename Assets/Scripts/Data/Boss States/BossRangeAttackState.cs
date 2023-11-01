using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRangeAttackState : BossAIState
{
    public override void EnterState(BossStateMachine stateMachine)
    {
    }

    public override void ExitState(BossStateMachine stateMachine)
    {
    }

    public override void UpdateState(BossStateMachine stateMachine)
    {
        stateMachine.Agent.SetDestination(stateMachine.TargetPlayer.position);
    }

}
