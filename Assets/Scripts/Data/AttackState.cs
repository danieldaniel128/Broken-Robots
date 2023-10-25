using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : AIState
{
    public override void EnterState(StateMachine stateMachine)
    {
        
    }

    public override void ExitState(StateMachine stateMachine)
    {
        
    }

    public override void UpdateState(StateMachine stateMachine)
    {
        if(Vector3.Distance(stateMachine.transform.position, stateMachine.Target.position) >= stateMachine.AttackRadius + 0.1f)
            stateMachine.ChangeState(stateMachine.AIStates.Find(c => c is ChaseState));

    }
}
