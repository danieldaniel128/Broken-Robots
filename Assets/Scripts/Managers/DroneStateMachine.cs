using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DroneStateMachine : MonoBehaviour
{
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float distanceFromPlayer = 5f;


    private void Update()
    {
        Vector3 targetPosition = playerTransform.position + (playerTransform.forward * distanceFromPlayer);
        RaycastHit hit;
        if (Physics.Raycast(targetPosition, Vector3.down, out hit))
        {
            targetPosition.y = hit.point.y + (GetComponent<Renderer>().bounds.extents.y * 2f); // Float above the ground
        }

        navMeshAgent.SetDestination(targetPosition);
    }
}
