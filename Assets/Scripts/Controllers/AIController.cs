using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] NavMeshAgent agent;


    [SerializeField] float _minSpeed;
    [SerializeField] float _maxSpeed;
    [SerializeField] float _acceleration;

    [SerializeField] float _scanRadius;
    [SerializeField] LayerMask _targetLayer;
    [SerializeField] bool _chaseTarget;//makes it easy for testing in editor

    Action OnSecondFrame;

    private void Start()
    {
        agent.speed = _minSpeed;
        agent.acceleration = float.MaxValue;//change the speed to the agent.speed
        OnSecondFrame += ChangeAcceleration;
    }

    void Update()
    {
        OnSecondFrame?.Invoke();
        ScanForTarget();
        MoveNavAgent();
    }

    /// <summary>
    /// changing from huge acceleration of the first frame to regular acceleration
    /// </summary>
    void ChangeAcceleration()
    {
        agent.acceleration = _acceleration;
        if(Time.frameCount == 2)
            agent.acceleration = _acceleration;
    }

    /// <summary>
    /// moves the nav agent
    /// </summary>
    void MoveNavAgent()
    {
        if (target == null)
        {
            agent.SetDestination(transform.position);
            return;
        }
        if (_chaseTarget)
        {
            Vector3 targetPos = new Vector3(target.position.x, target.position.y, transform.position.z);
            agent.SetDestination(targetPos);
        }
        else
            agent.SetDestination(transform.position);
    }

    /// <summary>
    /// scan for targets to chase 
    /// </summary>
    void ScanForTarget()
    {
        Collider[] potentailColliders = Physics.OverlapSphere(transform.position, _scanRadius, _targetLayer);
        target = GetClosestTarget(potentailColliders);
    }



    /// <summary>
    /// gets the closest target of the targets around the agent
    /// </summary>
    /// <param name="potentailColliders"></param>
    /// <returns></returns>
    Transform GetClosestTarget(Collider[] potentailColliders)
    {
        if (potentailColliders == null)
            return null;
        Transform nearestTarget = null;
        float nearestDistance = Mathf.Infinity;

        foreach (Collider collider in potentailColliders)
        {
            float distanceToTarget = Vector3.Distance(transform.position, collider.transform.position);
            if (distanceToTarget < nearestDistance)
            {
                nearestTarget = collider.transform;
                nearestDistance = distanceToTarget;
            }
        }
        return nearestTarget;
    }
}
