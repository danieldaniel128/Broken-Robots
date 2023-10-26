using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DroneStateMachine : MonoBehaviour
{
    [SerializeField] Transform target;//player
    public float _speed = 5f;
    [SerializeField] int numberOfRays;
    [SerializeField] float angle;
    [SerializeField] Vector3 RaysDirection;
    [SerializeField] float rayRange;


    [SerializeField] private Rigidbody droneRigidbody;

    private Vector3 targetDirection;
    private Vector3 patrolStartPoint;
    private Vector3[] patrolPoints;
    [SerializeField] float _patrolRange;
    private void Update()
    {
        targetDirection = target.position - transform.position;
        var deltaPosition = Vector3.zero;
        if (target == null)
        {
            return;
        }
        for (int i = 0; i < numberOfRays; i++)
        {
            Quaternion rotation = transform.rotation;
            Quaternion rotationMod = Quaternion.AngleAxis((i / ((float)numberOfRays - 1)) * angle - angle / 2f, Vector3.forward);
            var direction = rotation * rotationMod * targetDirection.normalized;
            Ray ray = new Ray(transform.position, direction);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, rayRange))
            {
                deltaPosition -= (1f / numberOfRays) * direction;
            }
            else
                deltaPosition += (1f / numberOfRays) * direction;
        }
        transform.position += deltaPosition * _speed * Time.deltaTime;
    }


    private void OnDrawGizmos()
    {
        Quaternion rotation = transform.rotation;

        for (int i = 0; i < numberOfRays; i++)
        {
            Quaternion rotationMod = Quaternion.AngleAxis((i / ((float)numberOfRays - 1)) * angle - angle / 2f, Vector3.forward);
            var direction = rotation * rotationMod * targetDirection.normalized * rayRange;
            Gizmos.DrawRay(transform.position, direction);
        }
    }

    void SetPatrol()
    {
        patrolStartPoint = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        SetPatrolPoints();
    }
    void SetPatrolPoints()
    {
        patrolPoints = new Vector3[2];
        patrolPoints[0] = patrolStartPoint + Vector3.up * _patrolRange;
        patrolPoints[1] = patrolStartPoint + Vector3.down * _patrolRange;
    }
}
