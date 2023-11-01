using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossStateMachine : MonoBehaviour
{
    public NavMeshAgent Agent;
    public List<BossAIState> AIStates;
    public BossAIState CurrentState { get; private set; }
    private void Start()
    {
        AIStates = new List<BossAIState>();
        AIStates.Add(new BossRangeAttackState());
    }

    private void Update()
    {
        CurrentState.UpdateState(this);
        RotateAgentTowardsDestination();
        //KeepsTheAgentOnZAxis();
    }

    public void ChangeState(BossAIState newState)
    {
        CurrentState.ExitState(this);
        CurrentState = newState;
        CurrentState.EnterState(this);
        Debug.Log("CurrentState: " + CurrentState);
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

    public void OnBossDeath()
    {

    }

    //private void KeepsTheAgentOnZAxis()
    //{
    //    Vector3 newPosition = transform.position;
    //    newPosition.z = originalZPosition;
    //    transform.position = newPosition;
    //}
}
