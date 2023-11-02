using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossStateMachine : MonoBehaviour
{
    public NavMeshAgent Agent;
    public List<BossAIState> AIStates;
    public BossAIState CurrentState { get; private set; }
    public Transform TargetPlayer;


    [SerializeField] GameObject _enemyChipPrefab;
    public BossAIState CurrentSummonPrefab { get; set; }
    public LayerMask BossWallLayer;
    [SerializeField] private float _distanceFromWall;
    [SerializeField] float _rotationSpeed;

    [SerializeField] private ProjectileSpawner _projectileSpawner;

    [SerializeField] private float _attackLazerCooldown;
    private float _attackLaserTimer;


    private void Start()
    {
        AIStates = new List<BossAIState>();
        AIStates.Add(new BossRangeAttackState());
        AIStates.Add(new BossSummonTaskchipState());
        CurrentState = AIStates.Find(c => c is BossRangeAttackState);
        CurrentState.EnterState(this);
    }

    private void Update()
    {
        CurrentState.UpdateState(this);
        RotateAgentTowardsDestination();
        MoveBossToCorners();
        AttackPlayer();
        ActivateLaserAttackCooldown();
        //KeepsTheAgentOnZAxis();
    }

    public void ChangeState(BossAIState newState)
    {
        CurrentState.ExitState(this);
        CurrentState = newState;
        CurrentState.EnterState(this);
        //Debug.Log("CurrentState: " + CurrentState);
    }
    private void RotateAgentTowardsDestination()
    {
        // Calculate the rotation angle based on the X-axis as forward and invert it.
        float angle = Mathf.Atan2(GetBossDirectionToPlayer().z, GetBossDirectionToPlayer().x) * Mathf.Rad2Deg;

        // Add 180 degrees to the angle to make it opposite.
        angle += 180f;
        //Debug.Log("angle: " + angle);

        // Create a Quaternion to represent the rotation.
        Quaternion targetRotation = Quaternion.Euler(0f, angle, 0f);

        // Apply the rotation to the agent smoothly using Slerp.
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
    }

    public void OnBossDeath()
    {

    }
    private void MoveBossToCorners()
    {
            Ray ray = new Ray(transform.position, GetBossDirectionToPlayer()); // Create a ray from the mouse position
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, BossWallLayer))
            {
                float distance = hit.distance;
                Agent.SetDestination(transform.position + GetBossDirectionToPlayer() * distance -GetBossDirectionToPlayer() * _distanceFromWall);
            }
    }

    private Vector3 GetBossDirectionToPlayer()
    {
        float bossDirectionToPlayer = TargetPlayer.position.x - transform.position.x;
        if (-bossDirectionToPlayer >= 0)
            return Vector3.right;
        else
            return Vector3.left;
    }
    

    bool hasLaserAttacked;
    void AttackPlayer()
    {
        if (hasLaserAttacked)
            return;
        ShootPlayer();
        hasLaserAttacked = true;
    }
    void ShootPlayer()
    {
        _projectileSpawner.SpawnProjectile(TargetPlayer.position - _projectileSpawner.transform.position);
    }
    void ActivateLaserAttackCooldown()
    {
        if (!hasLaserAttacked)
            return;
        if (_attackLaserTimer < _attackLazerCooldown)
            _attackLaserTimer += Time.deltaTime;
        else
        {
            hasLaserAttacked = false;
            _attackLaserTimer = 0;
        }
    }
}
