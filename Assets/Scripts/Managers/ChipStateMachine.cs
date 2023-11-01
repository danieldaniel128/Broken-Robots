using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class ChipStateMachine : MonoBehaviour
{
    public NavMeshAgent Agent;
    public List<ChipAIState> AIStates;
    public ChipAIState CurrentState { get; private set; }
    public Transform Target { get; set; }
    public Vector3[] PatrolPoints { get; private set; }
    public Vector3 PatrolStartPoint { get; private set; }



    [Header("Patrol Parameters")]
    [SerializeField] private float _patrolRange;
    public Vector3[] SpecificPatrolPoints;
    public bool IsUsingSpecificPatrolPoints;

    [Header("Scan Parameters")]
    public float ScanRadius;
    public float ChaseRadius;
    public float CurrentSearchRadius;
    public LayerMask TargetLayer;

    [Header("Physics Parameters")]
    [SerializeField] float _minSpeed;
    [SerializeField] float _maxSpeed;
    [SerializeField] float _acceleration;

    [Header("Attack Parameters")]
    public float AttackRadius;

    Action OnSecondFrame;
    private float originalZPosition;

    public bool IsDead = false;

    private void Start()
    {
        SetStateList();
        SetFirstCurrentState();
        InitMovementphysics();
        originalZPosition = transform.position.z; // Record the original Z position.
    }

    private void Update()
    {
        if (IsDead)
            return;
        OnSecondFrame?.Invoke();//event that happens on second frame only
        CurrentState.UpdateState(this);
        RotateAgentTowardsDestination();
        KeepsTheAgentOnZAxis();
    }

    public void ChangeState(ChipAIState newState)
    {
        CurrentState.ExitState(this);
        CurrentState = newState;
        CurrentState.EnterState(this);
        Debug.Log("CurrentState: " + CurrentState);
    }

    #region State Machine Init
    /// <summary>
    /// for now its only 2 points. this method generate the patrol points instead of us working hard to put patrol points ourselves.
    /// </summary>
    private void SetPatrolPoints()
    {
        if (IsUsingSpecificPatrolPoints)
        {
            PatrolPoints = SpecificPatrolPoints;
            return;
        }
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

    void SetFirstCurrentState()
    {
        CurrentState = AIStates.Find(c => c is ChipPatrolState);
        CurrentState.EnterState(this);
    }
    void SetPatrolState()
    {
        PatrolStartPoint = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        SetPatrolPoints();
        AIStates.Add(new ChipPatrolState(PatrolPoints));
    }
    void SetScanState()
    {
        AIStates.Add(new ChipScanState());
    }
    void SetAttackState()
    {
        AIStates.Add(new ChipAttackState());
    }
    void SetChaseState()
    {
        AIStates.Add(new ChipChaseState());
    }
    void SetStateList()
    {
        AIStates = new List<ChipAIState>();
        SetPatrolState();
        SetScanState();
        SetAttackState();
        SetChaseState();
    }

    #endregion


    void OnDrawGizmos()
    {
        if (PatrolPoints ==null || PatrolPoints.Length != 2)
            return;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(PatrolStartPoint + Vector3.right * _patrolRange, PatrolStartPoint + Vector3.left * _patrolRange);
        switch (CurrentState)
        {
            case ChipAttackState:
                Gizmos.color = Color.red;
                break;
            case ChipChaseState://#FFA500
                Gizmos.color = new Color(255f / 255f, 165 / 255f, 0 / 255f);
                break;
        }
       // Gizmos.DrawLine(transform.position, Vector3.right * CurrentSearchRadius * transform.rotation.eulerAngles.)

    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        if(!IsUsingSpecificPatrolPoints)
            Gizmos.DrawLine(transform.position + Vector3.right * _patrolRange, transform.position + Vector3.left * _patrolRange);
        else if(SpecificPatrolPoints != null && SpecificPatrolPoints.Length != 0)
            Gizmos.DrawLine(SpecificPatrolPoints[0], SpecificPatrolPoints[SpecificPatrolPoints.Length-1]);
    }

    private void RotateAgentTowardsDestination()
    {
        Vector3 destination = Agent.destination;
        Vector3 direction = (destination - transform.position).normalized;

        // Calculate the rotation angle based on the X-axis as forward and invert it.
        float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;

        // Create a Quaternion to represent the rotation.
        Quaternion targetRotation = Quaternion.Euler(0f, angle, 0f);

        // Apply the rotation to the agent.
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * Agent.angularSpeed);
    }
    private void KeepsTheAgentOnZAxis()
    {
        Vector3 newPosition = transform.position;
        newPosition.z = originalZPosition;
        transform.position = newPosition;
    }

    public void OnChipDeath()
    {
        IsDead = true;
        IsUsingSpecificPatrolPoints = true;
        SetPatrolPoints();
    }

}
