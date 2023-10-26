using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : AIState
{
    public override void EnterState(ChipStateMachine stateMachine)
    {
        stateMachine.Agent.SetDestination(stateMachine.Target.position);
    }

    public override void ExitState(ChipStateMachine stateMachine)
    {

    }

    public override void UpdateState(ChipStateMachine stateMachine)
    {
        stateMachine.Agent.SetDestination(stateMachine.Target.position);

        if (Vector3.Distance(stateMachine.transform.position, stateMachine.Target.position) <= stateMachine.AttackRadius + 0.1f)//change to attack if in attack radius
        {
            stateMachine.ChangeState(stateMachine.AIStates.Find(c => c is AttackState));
        }
        else
        {
            stateMachine.CurrentSearchRadius = stateMachine.ChaseRadius;
            stateMachine.ChangeState(stateMachine.AIStates.Find(c => c is ScanState));//change state to scan state
        }
    }
}
