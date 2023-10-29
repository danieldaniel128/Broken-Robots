using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Rigidbody body, levitation;

    [SerializeField] float levForce = 1, levMax = 2, levMin = .5f, movSpeed = 5, jumpImpulse = 1;
    float levMult = 1;
    bool canJump, jumpLock, isGrounded;
    const float jumpTimer = 1.5f;

    enum PlayerState { 
        Normal,
        Jumping,
        Airborne
    }

    PlayerState state;

    float dir;
    // Start is called before the first frame update
    void Start()
    {
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
         transform.position += dir * movSpeed * Time.deltaTime * Vector3.right;
    }
    private void LateUpdate()
    {
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
            if (state == PlayerState.Airborne)
            {
                state = PlayerState.Normal;
            }
            return Mathf.Lerp(levMax * 2f * levMult, levMin * levMult, hit.distance);
        }


    }

    public void Move(InputAction.CallbackContext ctx) {
        switch (ctx.phase) {
            case InputActionPhase.Performed: dir = ctx.ReadValue<float>(); break;
            default: dir = 0; break;
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
                    levitation.AddForce(jumpImpulse * Vector3.up, ForceMode.Impulse);
                    state = PlayerState.Airborne;
                    jumpLock = false;
                    canJump = false;
                    StartCoroutine(StartTimer(jumpTimer, () => canJump = true));
                    break;
                }
        }
    }
    public void Shock(InputAction.CallbackContext ctx) { }
    public void Purify(InputAction.CallbackContext ctx) { }
    public void Repair(InputAction.CallbackContext ctx) { }

    IEnumerator StartTimer(float time, UnityAction action)
    {
        yield return new WaitForSeconds(time);
        action();
        yield return null;
    }
}
