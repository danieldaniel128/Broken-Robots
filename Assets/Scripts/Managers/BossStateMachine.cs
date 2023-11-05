using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class BossStateMachine : MonoBehaviour
{
    public NavMeshAgent Agent;
    public Transform TargetPlayer;
    public List<BossAIState> AIStates { get; private set; }
    public BossAIState CurrentState { get; private set; }
    bool stopMove = false;
    Vector3 startPos;

    [SerializeField] Animator _bossAnimator;
    [SerializeField] GameObject _bossBody;
    [SerializeField] GameObject _playerRollingHitCollider;


    [Header("Boss Movement")]
    [SerializeField] private LayerMask _bossWallLayer;
    [SerializeField] private float _distanceFromWall;
    [SerializeField] private float _moveSpeed;
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

    [Header("Rolling Attack")]
    [SerializeField] float _corneredBossDistance;
    [SerializeField] float _rollingSpeed;
    [SerializeField] int _rollingDamage;
    bool _isRollingAttackOn = false;

    [Header("Purify Attack")]
    [SerializeField] float PurifyCooldown;

    List<GameObject> _summonsList;
    float PurifyTimer;

    float _currentSpeed;

    private void Start()
    {
        startPos = transform.position;
        _currentSpeed = _moveSpeed;
    }
    private void Update()
    {
        //CurrentState.UpdateState(this);

        //movement
        Agent.speed = _currentSpeed;
        if (!stopMove)
        {

            MoveBossToCorners();
            RollingAttackPlayer();
            StopRollingAttack();

            //lazer attack
            if (!_isRollingAttackOn)
            {
                LaserAttackPlayer();
                ActivateLaserAttackCooldown();
            }

            //summon attack
            //SummonAttackPlayer();
            //ActivateSummonAttackCooldown();

        }
        ActivatePurifyCooldown();
        RotateAgentTowardsDestination();
        GetBossTo0ZAxis();
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
        _bossBody.transform.rotation = Quaternion.Slerp(_bossBody.transform.rotation, targetRotation, Time.deltaTime * _rotationSpeed);
    }

    public void OnBossDeath()
    {
        stopMove = true;
        GetComponentInChildren<EnemyStatus>().IsDead = true;
        Agent.SetDestination(startPos);
    }
    private void MoveBossToCorners()
    {
        float distance = 0;
        Ray ray = new Ray(transform.position, GetBossDirectionToPlayer()); //
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _bossWallLayer))
        {
            distance = hit.distance;
            if(!_isRollingAttackOn)
            Agent.SetDestination(transform.position + GetBossDirectionToPlayer() * distance -GetBossDirectionToPlayer() * _distanceFromWall);
            
        }
     
    }


    #region LazerAttack
    void LaserAttackPlayer()
    {
        if (_hasLaserAttacked)
            return;
        ShootPlayer();
        _hasLaserAttacked = true;
    }
    void ShootPlayer()
    {
        _projectileSpawner.SpawnProjectile(TargetPlayer.position - _projectileSpawner.transform.position);
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
    #endregion

    #region SummonAttack
    void SummonAttackPlayer()
    {
        if (_hasSummonAttacked)
            return;
        SummonEnemies();
        DisactivateSummons();
        _hasSummonAttacked = true;
    }
    void SummonEnemies()
    {
        if (GetBossDirectionToPlayer().Equals(Vector3.left))
        {
            _currentSummonPrefab = _enemyBasicChipPrefab;
            _currentSummonSpawnPos = _spawnSummonPositionRightWall;
            //Debug.Log("spawn from right");
        }
        else
        {
            _currentSummonPrefab = _enemyHighSpeedChip;
            _currentSummonSpawnPos = _spawnSummonPositionLeftWall;
            //Debug.Log("spawn from left");
        }
        _summonsList.Add(Instantiate(_currentSummonPrefab, _currentSummonSpawnPos.position, Quaternion.identity, _currentSummonSpawnPos));
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
    void DisactivateSummons()
    {
        if (_summonsList == null)
            return;
        if(_isRollingAttackOn)
        foreach (GameObject summon in _summonsList)
        {
            ChipStateMachine chipStateMachine = summon.GetComponent<ChipStateMachine>();
            chipStateMachine.ChipCollider.enabled = false;
            chipStateMachine.ChipTrigger.enabled = false;
            chipStateMachine.Agent.enabled = false;
            chipStateMachine.enabled = false;
        }
    }

    void ActivateSummons()
    {
        if (_summonsList == null)
            return;
        foreach (GameObject summon in _summonsList)
        {
            ChipStateMachine chipStateMachine = summon.GetComponent<ChipStateMachine>();
            chipStateMachine.Agent.enabled = true;
            chipStateMachine.enabled = true;
            chipStateMachine.ChipCollider.enabled = true;
            chipStateMachine.ChipTrigger.enabled = true;
        }
    }
    #endregion

    void ActivatePurifyCooldown()
    {
        if (!stopMove)
            return;
        if (PurifyTimer < PurifyCooldown)
            PurifyTimer += Time.deltaTime;
        else
        {
            if(!GetComponent<EnemyStatus>().IsPurify)
            {
                GetComponent<EnemyStatus>().IsDead = false;
                stopMove = false;
                GetComponent<Health>().CurrentHealth = 3;
            }
            PurifyTimer = 0;
        }
    }

    #region Rolling Attack

    void RollingAttackPlayer()
    {
        if (!_isRollingAttackOn)//if in not in mid rolling
            if (Agent.remainingDistance <= 0.2f)
                if (Vector3.Distance(transform.position, TargetPlayer.position) <= _corneredBossDistance)//checks if got to a corner and in the player got close to boss.
                {
                    Ray ray = new Ray(transform.position, -GetBossDirectionToPlayer()); //
                    if (Physics.Raycast(ray, out RaycastHit hit2, Mathf.Infinity, _bossWallLayer))
                    {
                        float distance = hit2.distance;
                        Debug.Log("time to roll");
                        RollingPlayer(distance);
                        DisactivateSummons();
                    }
                }
    }
    void StopRollingAttack()
    {
        if (_isRollingAttackOn)
            if (Agent.remainingDistance <= 0.2f)//got to other corner after rolling
            {
                _isRollingAttackOn = false;
                _currentSpeed = _moveSpeed;
                Debug.Log("finished rolling");
                _bossAnimator.SetBool("IsRolling", _isRollingAttackOn);
                ActivateSummons();
            }
    }
    void RollingPlayer(float wallDistance)
    {
        if (_isRollingAttackOn)
            return;
        Debug.Log("start rolling");
        Agent.SetDestination(transform.position - GetBossDirectionToPlayer() * wallDistance + GetBossDirectionToPlayer() * _distanceFromWall);
        _isRollingAttackOn = true;
        _currentSpeed = _rollingSpeed;
        _playerRollingHitCollider.SetActive(true);
        _bossAnimator.SetBool("IsRolling", _isRollingAttackOn);
    }
    #endregion

    #region Utility
    void GetBossTo0ZAxis()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }
    private Vector3 GetBossDirectionToPlayer()
    {
        float bossDirectionToPlayer = TargetPlayer.position.x - transform.position.x;
        if (-bossDirectionToPlayer >= 0)
            return Vector3.right;
        else
            return Vector3.left;
    }
    #endregion
    private void OnTriggerEnter(Collider other)
    {
        if(_isRollingAttackOn && other.tag.Equals("RollingHitBoxPlayer"))
        if (other.GetComponentInParent<PlayerController>() != null)
        {
            other.GetComponentInParent<Health>().TakeDamage(_rollingDamage);
            _playerRollingHitCollider.SetActive(false);
            Debug.Log("boss hit player rolling");
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(Agent.destination, 1);
    }
}
