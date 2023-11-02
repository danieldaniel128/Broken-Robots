using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class PlayerController : MonoBehaviour
{
    struct Mana
    {
        public float maxMana;
        public float curMana{ 
            get { return _curMana; }
            set { _curMana = value; 
                onManaChange?.Invoke(value);  } }
        float _curMana;

        public UnityEvent<float> onManaChange;
    }

    [SerializeField] GameObject _dashParticles;
    [SerializeField] GameObject _playerBodyModel;
    [SerializeField] Rigidbody body, levitation;

    [SerializeField] float levForce = 1, levMax = 2, levMin = .5f, movSpeed = 5, jumpImpulse = 1, jumpChargeRate = 1, dashSpeed = 1, dashRange = 1;
    float levMult = 1, speedMult = 1, jumpForce, manaDrain = 0;
    bool canJump, jumpLock, dashAvailable, movementLock;
    bool rememberDash;
    const float jumpTimer = .5f;
    [SerializeField]TextMeshProUGUI stateGUI;

    [Header("Resource"), SerializeField] int maxMana;
    [SerializeField]float currentMana;
    public UnityEvent<float> onManaChange;

    [Header("Purify"), SerializeField] float radius;
    [SerializeField] int pulseStartFrames, pulseActiveFrames, pulseRecoveryFrames;
    EnemyStatus target;
    Coroutine purify;
    PlayerInput input;

    [Header("Repair"), SerializeField] float duration;
    [SerializeField]float drainAmount;
    [SerializeField] int healAmount;
    enum PlayerState { 
        Normal,
        Jumping,
        Airborne
    }

    PlayerState state;
    Shock shock;
    Health health;
    Mana mana;

    float inputDir, lookDir = 1;

    // InputBinding inputBinding = action.bindings[0];
    // inputBinding.overridePath = path;
    // action.ApplyBindingOverride(0, inputBinding);


    private void Awake()
    {
        shock = GetComponentInChildren<Shock>(true);
        input = GetComponent<PlayerInput>();

        InputAction repair = input.actions["Repair"];
        InputBinding inputBind = repair.bindings[0];
        inputBind.overrideInteractions = $"Hold(duration={duration})";
        repair.ApplyBindingOverride(0,inputBind);
        
        target = null;
        health = GetComponent<Health>();
        mana = new Mana { maxMana = maxMana, curMana = 0, onManaChange = new UnityEvent<float>() };
        mana.onManaChange.AddListener(onManaChange.Invoke);
        mana.onManaChange.AddListener(val => currentMana = val);
        mana.curMana = maxMana;
    }


    void Start()
    {
        movementLock = false;
        jumpLock = false;
        canJump = true;
        state = PlayerState.Normal;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float df = DistanceFloor();
        Levitate(df);
    }

    private void Update()
    {
        transform.position += inputDir * movSpeed  * speedMult * Time.deltaTime * Vector3.right;
        if (manaDrain != 0 && mana.curMana > 0)
        {
            mana.curMana -= manaDrain * Time.deltaTime;
            mana.curMana = mana.curMana > 0 ? mana.curMana : 0;
        }


        if (jumpLock)
        {
            jumpForce += Time.deltaTime * jumpChargeRate;
            jumpForce = jumpForce > 1 ? 1 : jumpForce;
        }
        RotateAgentTowardsDestination();
    }
    private void LateUpdate()
    {
        if (stateGUI is not null)
            stateGUI.text = state.ToString();
    }

    void Levitate(float distanceFloor) {
        levitation.AddForce(distanceFloor * levForce * levMult * Vector3.up);
    }

    float DistanceFloor()
    {
        Ray down = new(levitation.position, Vector3.down);

        if ( ! Physics.Raycast(down, out RaycastHit hit, levMax))
        {
            return 0;
        } else
        {
            if (!movementLock)
            {
                dashAvailable = true;
            }
            if (state == PlayerState.Airborne && canJump)
            {
                state = PlayerState.Normal;
            }
            return hit.collider.gameObject.tag == "Spikes" ? 0 : Mathf.Lerp(levMax * 2f, levMin, hit.distance);
        }


    }

    public void Move(InputAction.CallbackContext ctx) {
        switch (ctx.phase) {
            case InputActionPhase.Performed: lookDir = inputDir = ctx.ReadValue<float>(); break;
            default: inputDir = 0; break;
        }
    }

    public void Jump(InputAction.CallbackContext ctx) {

        if (!canJump || state == PlayerState.Airborne) return;
        
        switch (ctx.phase)
        {
            case InputActionPhase.Started:
                {
                    jumpLock = true;
                    state = PlayerState.Jumping;
                    jumpForce = 0;
                    break;
                }
            case InputActionPhase.Performed:
                {
                    if (!jumpLock) return;
                    levMult = .5f;
                    break;
                }
            case InputActionPhase.Canceled:
                {
                    if (!jumpLock) return;
                    levMult = 1;
                    levitation.AddForce(jumpImpulse * jumpForce * Vector3.up, ForceMode.Impulse);
                    state = PlayerState.Airborne;
                    jumpLock = false;
                    canJump = false;
                    StartCoroutine(StartTimer(jumpTimer, () => canJump = true));
                    break;
                }
        }
    }

    public void Dash(InputAction.CallbackContext ctx)
    {
        if (!dashAvailable) return;
        switch (ctx.phase)
        {
            case InputActionPhase.Started: LockGravity(true); movementLock = true; _dashParticles.SetActive(true); _playerBodyModel.SetActive(false) ; StartCoroutine(StopDash(1)); break;
            case InputActionPhase.Performed:
                {
                    dashAvailable = false;
                    float targetX = transform.position.x + dashRange * inputDir;

                    StartCoroutine(WaitCondition(() => (transform.position.x - targetX) * Mathf.Sign(inputDir) >= 0, () => { speedMult = 0; StartCoroutine(WaitFrames(5, () => { speedMult = 1; movementLock = false; LockGravity(false); })); }));
                    speedMult = dashSpeed;
                    break;
                }
        }
    }

    void ActionLock(bool isLock)
    {
        if (isLock)
        {
            rememberDash = dashAvailable;
        }
        dashAvailable = isLock ? false : rememberDash;
        movementLock = isLock;
        LockGravity(isLock);
        speedMult = isLock ? 0 : 1;
        levMult = isLock ? 0 : 1;
    }

    public void Shock(InputAction.CallbackContext ctx) {
        if (!shock.isAvailable) return;
        switch (ctx.phase)
        {
            case InputActionPhase.Started:
                {
                    shock.SetDirection(lookDir);
                    break;
                }
            case InputActionPhase.Performed:
                {
                    ActionLock(true);
                    StartCoroutine(shock.Perform(this, () =>  ActionLock(false)));
                    break;
                }
        }
    }
    public void Purify(InputAction.CallbackContext ctx)
    {
        switch (ctx.phase)
        {
            case InputActionPhase.Started:
                {
                    Debug.Log("Looking for Purify Target");
                    target = Physics.OverlapSphere(body.position, radius)
                         .OrderBy(c => Vector3.Distance(body.position, c.transform.position))
                         .FirstOrDefault(c => c.TryGetComponent<EnemyStatus>(out EnemyStatus enemy) && enemy.IsDead && !enemy.IsPurify)?.GetComponent<EnemyStatus>();
                    break;
                }
            case InputActionPhase.Performed:
                {
                    if (target is not null)
                    {
                        Debug.Log($"Purifying {target.gameObject.name}");
                        ActionLock(true);
                        purify = StartCoroutine(PurifyRoutine(target));
                    }
                    break;
                }
            default: 
                {
                    if (purify is not null) 
                    { 
                        StopCoroutine(purify); 
                        purify = null;
                        ActionLock(false);
                        Debug.Log($"Purifying status - {target.IsPurify}");
                    }
                    break; 
                }
        }
    }
    
    IEnumerator PurifyRoutine(EnemyStatus target)
    {
        yield return WaitFrames(pulseStartFrames);
        yield return WaitFrames(pulseActiveFrames);
        target.IsPurify = true;
        yield return WaitFrames(pulseRecoveryFrames);
        ActionLock(false);
        yield return null;
    }

    public void Repair(InputAction.CallbackContext ctx) {
        Debug.Log(health.isMaxed);
        void handle(InputAction.CallbackContext ctx) => ChangeDrain(-drainAmount);
        switch (ctx.phase)
        {
            case InputActionPhase.Waiting: ctx.action.canceled -= handle; break;
            case InputActionPhase.Started: if (mana.curMana == 0 || health.isMaxed) return; ActionLock(true); ChangeDrain(drainAmount); ctx.action.canceled += handle ; break;
            case InputActionPhase.Performed:
                {
                    if (health.isMaxed) return;
                    if (mana.curMana > 0)
                        health.Heal(healAmount);
                    handle(ctx);
                    ctx.action.canceled -= handle;
                    ActionLock(false);
                    break;
                }
            case InputActionPhase.Canceled: ActionLock(false); break;
        }

    }

    void ChangeDrain(float amount)
    {
        manaDrain += amount;
    }

    public IEnumerator StartTimer(float time, UnityAction action = null)
    {
        yield return new WaitForSeconds(time);
        action?.Invoke();
        yield return null;
    }

    public IEnumerator WaitCondition(Func<bool> condition, UnityAction action = null)
    {
        while (!condition())
        {
            yield return new WaitForEndOfFrame();
        }
        action?.Invoke();
        yield return null;
    }

    public IEnumerator WaitFrames(int frameNum, UnityAction action = null)
    {
        while (frameNum > 0)
        {
            yield return new WaitForEndOfFrame();
            frameNum--;
        }
        action?.Invoke();
        yield return null;
    }

    void LockGravity(bool isLock)
    {
        isLock ^= true; // invert boolean
        levitation.useGravity = body.useGravity = isLock;
        levitation.velocity = body.velocity = Vector3.zero;
    }

    public IEnumerator StopDash(float time)
    {
        yield return new WaitForSeconds(time);
        _dashParticles.SetActive(false);
        _playerBodyModel.SetActive(true);

    }


    private void RotateAgentTowardsDestination()
    {
        levitation.position = new Vector3(levitation.position.x, levitation.position.y, 0);
        body.position = new Vector3(body.position.x, body.position.y, 0);
        // Calculate the rotation angle based on the X-axis as forward and invert it.
        float angle = Mathf.Atan2(0, -lookDir) * Mathf.Rad2Deg;

        // Add 180 degrees to the angle to make it opposite.
        angle += 180f;
        //Debug.Log("angle: " + angle);

        // Create a Quaternion to represent the rotation.
        Quaternion targetRotation = Quaternion.Euler(0f, angle, 0f);

        // Apply the rotation to the agent smoothly using Slerp.
        body.rotation = Quaternion.Slerp(body.rotation, targetRotation, Time.deltaTime * 5);
    }
}
