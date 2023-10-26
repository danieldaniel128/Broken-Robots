using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ChipPatrolState : AIState
{
    private Vector3[] _patrolPoints;
    private int _currentPatrolIndex => _patrolCounter % 2 ;
    private int _patrolCounter;

    public ChipPatrolState(Vector3[] points)
    {
        _patrolPoints = points;
        _patrolCounter = 0;
    }

    public override void EnterState(ChipStateMachine stateMachine)
    {
        //set the first destination before the update state event 
        stateMachine.Agent.SetDestination(_patrolPoints[_currentPatrolIndex]);
    }

    public override void ExitState(ChipStateMachine stateMachine)
    {
    }

    public override void UpdateState(ChipStateMachine stateMachine)
    {
        //if distance from patrol point is smaller than the stop distance, it has reach. the 0.1 is for aproximatly reach the point
        if (Vector3.Distance(stateMachine.transform.position, _patrolPoints[_currentPatrolIndex]) <= stateMachine.Agent.stoppingDistance + 0.1f)
        {
            _patrolCounter++;
            stateMachine.Agent.SetDestination(_patrolPoints[_currentPatrolIndex]);
        }
        stateMachine.ChangeState(stateMachine.AIStates.Find(c => c is ChipScanState));
    }


}
