using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossStateMachine : MonoBehaviour
{
    public NavMeshAgent Agent;
    public Transform TargetPlayer;
    public List<BossAIState> AIStates { get; private set; }
    public BossAIState CurrentState { get; private set; }


    [Header("Boss Movement")]
    [SerializeField] private LayerMask _bossWallLayer;
    [SerializeField] private float _distanceFromWall;
    [SerializeField] float _rotationSpeed;



    [Header("Summon Ability")]
    [SerializeField] private GameObject _enemyBasicChipPrefab;
    [SerializeField] private GameObject _enemyHighSpeedChip;
    [SerializeField] private Transform _spawnSummonPositionLeftWall;
    [SerializeField] private Transform _spawnSummonPositionRightWall;
    [SerializeField] private float _summonAttackCooldown;
    //attack timer
    private float _summonAttackTimer;
    bool _hasSummonAttacked;

    private GameObject _currentSummonPrefab;
    private Transform _currentSummonSpawnPos;





    [Header("Lazer Attack")]
    [SerializeField] private ProjectileSpawner _projectileSpawner;
    [SerializeField] private float _lazerAttackCooldown;
    private float _laserAttackTimer;
    bool _hasLaserAttacked;


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
        //CurrentState.UpdateState(this);

        //movement
        RotateAgentTowardsDestination();
        MoveBossToCorners();

        //lazer attack
        LaserAttackPlayer();
        ActivateLaserAttackCooldown();

        //summon attack
        SummonAttackPlayer();
        ActivateSummonAttackCooldown();
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
            Ray ray = new Ray(transform.position, GetBossDirectionToPlayer()); //
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _bossWallLayer))
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
    

    void LaserAttackPlayer()
    {
        if (_hasLaserAttacked)
            return;
        ShootPlayer();
        _hasLaserAttacked = true;
    }

    void SummonAttackPlayer()
    {
        if (_hasSummonAttacked)
            return;
        SummonEnemies();
        _hasSummonAttacked = true;
    }


    void ShootPlayer()
    {
        _projectileSpawner.SpawnProjectile(TargetPlayer.position - _projectileSpawner.transform.position);
    }

    void SummonEnemies()
    {
        if (GetBossDirectionToPlayer() == Vector3.right)
        {
            _currentSummonPrefab = _enemyBasicChipPrefab;
            _currentSummonSpawnPos = _spawnSummonPositionLeftWall;
            Debug.Log("spawn from right");
        }
        Instantiate(_currentSummonPrefab, _currentSummonSpawnPos.position, Quaternion.identity, _currentSummonSpawnPos);
    }
    void ActivateLaserAttackCooldown()
    {
        if (!_hasLaserAttacked)
            return;
        if (_laserAttackTimer < _lazerAttackCooldown)
            _laserAttackTimer += Time.deltaTime;
        else
        {
            _hasLaserAttacked = false;
            _laserAttackTimer = 0;
        }
    }
    void ActivateSummonAttackCooldown()
    {
        if (!_hasSummonAttacked)
            return;
        if (_summonAttackTimer < _summonAttackCooldown)
            _summonAttackTimer += Time.deltaTime;
        else
        {
            _hasSummonAttacked = false;
            _summonAttackTimer = 0;
        }
    }
}
