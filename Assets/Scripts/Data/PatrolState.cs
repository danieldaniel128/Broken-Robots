using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : AIState
{
    private Vector3[] _patrolPoints;
    private int _currentPatrolIndex => _patrolCounter % 2 ;
    private int _patrolCounter;

    public PatrolState(Vector3[] points)
    {
        _patrolPoints = points;
        _patrolCounter = 0;
    }

    public override void EnterState(StateMachine stateMachine)
    {
        //set the first destination before the update state event 
        stateMachine.Agent.SetDestination(_patrolPoints[_currentPatrolIndex]);
    }

    public override void ExitState(StateMachine stateMachine)
    {
    }

    public override void UpdateState(StateMachine stateMachine)
    {
        //if distance from patrol point is smaller than the stop distance, it has reach. the 0.5 is for aproximatly reach the point
        if (Vector3.Distance(stateMachine.transform.position, _patrolPoints[_currentPatrolIndex]) <= stateMachine.Agent.stoppingDistance + 0.5f)
        {
            _patrolCounter++;
            stateMachine.Agent.SetDestination(_patrolPoints[_currentPatrolIndex]);
        }
    }
}
