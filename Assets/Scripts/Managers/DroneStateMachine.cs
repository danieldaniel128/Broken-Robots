using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DroneStateMachine : MonoBehaviour
{
    public Transform target;
    public float enemySpeed = 5f;
    public float raycastDistance = 2f;

    [SerializeField] private Rigidbody droneRigidbody;

    private void FixedUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 direction = (target.position - transform.position).normalized;

        // Raycast to detect obstacles
        Ray ray = new Ray(transform.position, direction);
        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
        {
            // Calculate a new direction away from the obstacle
            Vector3 avoidanceDirection = Vector3.Reflect(direction, hit.normal).normalized;
            direction = avoidanceDirection;
        }

        // Apply movement
        droneRigidbody.velocity = direction * enemySpeed;
    }
}
