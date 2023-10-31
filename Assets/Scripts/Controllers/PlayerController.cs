using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Rigidbody body, levitation;

    [SerializeField] float levForce = 1, levMax = 2, levMin = .5f, movSpeed = 5, jumpImpulse = 1, jumpChargeRate = 1, dashSpeed = 1, dashRange = 1;
    float levMult = 1, speedMult = 1, jumpForce;
    bool canJump, jumpLock, isGrounded, dashAvailable, movementLock;
    const float jumpTimer = .5f;

    [SerializeField]TextMeshProUGUI stateGUI;

    enum PlayerState { 
        Normal,
        Jumping,
        Airborne
    }

    PlayerState state;
    Shock shock;

    float inputDir;

    private void Awake()
    {
        shock = GetComponentInChildren<Shock>(true);
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

        if (jumpLock)
        {
            jumpForce += Time.deltaTime * jumpChargeRate;
            jumpForce = jumpForce > 1 ? 1 : jumpForce;
        }
    }
    private void LateUpdate()
    {
        if (stateGUI is not null)
            stateGUI.text = state.ToString();
    }

    void Levitate(float distanceFloor) {
        levitation.AddForce(distanceFloor * levForce * Vector3.up);
    }

    float DistanceFloor()
    {
        Ray down = new(levitation.position, Vector3.down);

        if ( ! Physics.Raycast(down, out RaycastHit hit, levMax))
        {
            isGrounded = false;
            return 0;
        } else
        {
            isGrounded = true;
            if (!movementLock)
            {
                dashAvailable = true;
            }
            if (state == PlayerState.Airborne && canJump)
            {
                state = PlayerState.Normal;
            }
            return hit.collider.gameObject.tag == "Spikes" ? 0 : Mathf.Lerp(levMax * 2f * levMult, levMin * levMult, hit.distance);
        }


    }

    public void Move(InputAction.CallbackContext ctx) {
        switch (ctx.phase) {
            case InputActionPhase.Performed: inputDir = ctx.ReadValue<float>(); break;
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
            case InputActionPhase.Started: LockGravity(true); movementLock = true;  break;
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

    public void Shock(InputAction.CallbackContext ctx) { 
     if (ctx.phase == InputActionPhase.Performed)
        {
            movementLock = true;
            LockGravity(true);
            StartCoroutine(shock.Perform(this, () => {
                movementLock = false;
                LockGravity(false);
            }));
        }
    }
    public void Purify(InputAction.CallbackContext ctx) { }
    public void Repair(InputAction.CallbackContext ctx) { }

    public IEnumerator StartTimer(float time, UnityAction action = null)
    {
        yield return new WaitForSeconds(time);
        action();
        yield return null;
    }

    public IEnumerator WaitCondition(Func<bool> condition, UnityAction action = null)
    {
        while (!condition())
        {
            yield return new WaitForEndOfFrame();
        }
        action();
        yield return null;
    }

    public IEnumerator WaitFrames(int frameNum, UnityAction action = null)
    {
        while (frameNum > 0)
        {
            yield return new WaitForEndOfFrame();
            frameNum--;
        }
        action();
        yield return null;
    }

    void LockGravity(bool isLock)
    {
        isLock ^= true; // invert boolean
        levitation.useGravity = body.useGravity = isLock;
        levitation.velocity = body.velocity = Vector3.zero;
    }
}
