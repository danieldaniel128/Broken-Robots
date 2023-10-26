using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ChipScanState : AIState
{
    private GameObject gameObject;
    public override void EnterState(ChipStateMachine stateMachine)
    {
        gameObject = stateMachine.gameObject;
    }

    public override void ExitState(ChipStateMachine stateMachine)
    {

    }

    public override void UpdateState(ChipStateMachine stateMachine)
    {
        ScanForTarget(stateMachine);
        if (stateMachine.Target != null)
        {
            stateMachine.ChangeState(stateMachine.AIStates.Find(c => c is ChipChaseState));//found a target, its time to chase it
        }
        else
        {
            stateMachine.ChangeState(stateMachine.AIStates.Find(c => c is ChipPatrolState));//didnt found, continue patrol
        }
    }

    /// <summary>
    /// scan for targets to chase 
    /// </summary>
    public void ScanForTarget(ChipStateMachine stateMachine)
    {
        Collider[] potentailColliders = Physics.OverlapSphere(gameObject.transform.position, stateMachine.CurrentSearchRadius, stateMachine.TargetLayer);
        stateMachine.Target = GetClosestTarget(potentailColliders);
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
            float distanceToTarget = Vector3.Distance(gameObject.transform.position, collider.transform.position);
            if (distanceToTarget < nearestDistance)
            {
                nearestTarget = collider.transform;
                nearestDistance = distanceToTarget;
            }
        }
        return nearestTarget;
    }
}
