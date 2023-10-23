using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class StateMachine : MonoBehaviour
{
    public NavMeshAgent Agent;
    public AIState CurrentState { get; private set; }
    public Vector3[] PatrolPoints { get; private set; }
    public Vector3 PatrolStartPoint { get; private set; }

    [SerializeField] private float _patrolRange;

    [Header("physics parameters")]
    [SerializeField] float _minSpeed;
    [SerializeField] float _maxSpeed;
    [SerializeField] float _acceleration;


    Action OnSecondFrame;

    private void Start()
    {
        SetFirstState();
        InitMovementphysics();
    }

    private void Update()
    {
        OnSecondFrame?.Invoke();//event that happens on second frame only
        CurrentState.UpdateState(this);
    }

    public void ChangeState(AIState newState)
    {
        CurrentState.ExitState(this);
        CurrentState = newState;
        CurrentState.EnterState(this);
    }

    #region State Machine Init
    /// <summary>
    /// for now its only 2 points. this method generate the patrol points instead of us working hard to put patrol points ourselves.
    /// </summary>
    private void SetPatrolPoints()
    {
        PatrolPoints = new Vector3[2];
        PatrolPoints[0] = PatrolStartPoint + Vector3.right * _patrolRange;
        PatrolPoints[1] = PatrolStartPoint + Vector3.left * _patrolRange;
    }

    /// <summary>
    /// changing from huge acceleration of the first frame to regular acceleration
    /// </summary>
    void ChangeAcceleration()
    {
        Agent.acceleration = _acceleration;
        if (Time.frameCount == 2)
            Agent.acceleration = _acceleration;
    }
    void InitMovementphysics()
    {
        Agent.speed = _minSpeed;
        Agent.acceleration = float.MaxValue;//change the speed to the agent.speed
        OnSecondFrame += ChangeAcceleration;
    }

    void SetFirstState()
    {
        PatrolStartPoint = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        SetPatrolPoints();
        CurrentState = new PatrolState(PatrolPoints);
        CurrentState.EnterState(this);

    }
    #endregion
}
