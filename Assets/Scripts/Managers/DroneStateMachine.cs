using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class DroneStateMachine : MonoBehaviour
{

    [SerializeField] Transform target;//player

    [Header("chase player parameters")]
    [SerializeField] float chaseTargetRange;
    [SerializeField] float _speed = 5f;
    bool isChasing;

    [Header("Patrol Parameters")]
    [SerializeField] float patrolTime;
    [SerializeField] float _patrolRange;
    private Vector3 targetDirection;
    private Vector3 patrolStartPoint;
    private Vector3[] patrolPoints;

    [Header("Attack Parameters")]
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown;
    private float attackTimer;
    private bool isAttacking;

    [Header("path finding parameters")]
    [SerializeField] int numberOfRays;
    [SerializeField] float angle;
    [SerializeField] float rayRange;

    [SerializeField] private Rigidbody droneRigidbody;

    [SerializeField] private ProjectileSpawner _projectileSpawner;

    int currentPatrolPoint => patrolCounter % 2;
    int patrolCounter;

    private void Start()
    {
        SetPatrol();
    }
    private void Update()
    {
        Patrol();
        ScanForTarget();
        ChasePlayer();
        AttackPlayer();
        ActivateAttackCooldown();
    }

    void ScanForTarget()
    {
        if (Vector3.Distance(target.position, transform.position) <= chaseTargetRange)
        {
            isChasing = true;
        }
        else
            isChasing = false;
        if (Vector3.Distance(target.position, transform.position) <= attackRange)
        {
            isAttacking = true;
        }
        else
            isAttacking = false;
    }

    void Patrol()
    {
        if (isChasing)
            return;
        transform.position = Vector3.MoveTowards(transform.position, patrolPoints[currentPatrolPoint], _patrolRange * (Time.deltaTime / patrolTime) * 2);
        if (transform.position == patrolPoints[currentPatrolPoint])
        {
            patrolCounter++;
        }
    }

    void ChasePlayer()
    {
        if (!isChasing || isAttacking)
            return;
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
    bool hasAttacked;
    void AttackPlayer()
    {
        if (!isAttacking || hasAttacked)
            return;
        ShootPlayer();
        hasAttacked = true;
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

    void ShootPlayer()
    {
        _projectileSpawner.SpawnProjectile(targetDirection);
    }
    void ActivateAttackCooldown()
    {
        if (!hasAttacked)
            return;
        if(attackTimer < attackCooldown)
            attackTimer += Time.deltaTime;
        else
        {
            hasAttacked = false;
            attackTimer = 0;
        }
    }
   
}
