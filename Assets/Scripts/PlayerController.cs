using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float airWalkSpeed = 3f;
    Vector2 moveInput;
    TouchingDirections touchingDirections;
    Damageable damageable;

    public float jumpImpulse = 10f;
    public bool doubleJumpAvailable = true;
    public bool usedJumpWhileAir = false;

    [Header("Gravity")]
    public float baseGravity = 1f;
    public float maxFallSpeed = 6f;
    public float fallSpeedMultiplier = 1.2f;

    public float dashDuration = 0.5f;
    public float dashSpeed = 40f;
    private TrailRenderer trailRenderer;
    private bool dashEnabled = true;
    private bool isDashing = false;

    public float CurrentMoveSpeed
    {
        get
        {
            if (CanMove && !isDashing)
            {
                if (IsMoving && !touchingDirections.isOnWall)
                {
                    if (touchingDirections.isGrounded)
                    {
                        if (IsRunning)
                        {
                            return runSpeed;
                        }
                        else
                        {
                            return walkSpeed;
                        }
                    }
                    else
                    {
                        return airWalkSpeed;
                    }
                }
                else
                {
                    return airWalkSpeed;
                }
            }
            else if (isDashing)
            {
                return dashSpeed;
            }
            else
            {
                // Movement locked
                return 0;
            }
        }
    }

    [SerializeField]
    private bool _isMoving = false;

    public bool IsMoving { get
        {
            return _isMoving;
        } private set { 
        
            _isMoving = value;
            animator.SetBool(AnimationString.isMoving, value);
        } }

    [SerializeField]
    private bool _isRunning = false;

    public bool IsRunning
    {
        get
        {
            return _isRunning;
        }
        set
        {
            _isRunning = value;
            animator.SetBool(AnimationString.isRunning, value);
        }
    }

    public bool _isFacingRight = true;
    public bool IsFacingRight { get{ return _isFacingRight; } private set
        {
            
            if (_isFacingRight != value)
            {
                transform.localScale *= new Vector2(-1, 1);
            }

            _isFacingRight = value;
        }
    }

    public bool CanAirAttack
    {
        get
        {
            return animator.GetBool(AnimationString.usedAirAttack);
        }
    }

    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationString.canMove);
        }
    }

    public bool IsAlive
    {
        get
        {
            return animator.GetBool(AnimationString.isAlive);
        }
    }

    Rigidbody2D rb;
    Animator animator;
    private float dashCooldown = 2f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        damageable = GetComponent<Damageable>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    private void FixedUpdate()
    {
        Gravity();
        if (!damageable.LockVelocity)
            
            rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);
            
        animator.SetFloat(AnimationString.yVelocity, rb.velocity.y);
        if (touchingDirections.isGrounded)
        {
            doubleJumpAvailable = true;
            usedJumpWhileAir = false;
            animator.SetBool(AnimationString.usedAirAttack, false);
        }

    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (IsAlive)
        {
            IsMoving = moveInput != Vector2.zero;
            SetFacingDirection(moveInput);
        }
        else
        {
            IsMoving = false;
        }
    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        if (moveInput.x > 0 && !IsFacingRight)
        {
            IsFacingRight = true;
        }
        else if(moveInput.x < 0 && IsFacingRight)
        {
            IsFacingRight = false;
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsRunning = true;
        }
        else if (context.canceled)
        {
            IsRunning = false;
        }
    }

    private void Gravity()
    {
        if(rb.velocity.y <= 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMultiplier;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // Reset jump when touching the ground
        // First jump logic (grounded)
        if (context.performed && touchingDirections.isGrounded && CanMove)
        {
            PerformJump();
        }
        else if(context.canceled && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

        // Double jump logic (in air)
        else if (context.started && !touchingDirections.isGrounded 
            && !touchingDirections.isOnCeiling 
            && !touchingDirections.isOnWall
            && doubleJumpAvailable && !usedJumpWhileAir && CanMove)
        {
            usedJumpWhileAir = true;
            doubleJumpAvailable = false;
            PerformJump();
        }
        // Single jump regained after falling from platform
        else if (context.started && !touchingDirections.isGrounded && !usedJumpWhileAir && CanMove)
        {
            usedJumpWhileAir = true;
            PerformJump();
        }
    }
    private void PerformJump()
    {
        animator.SetTrigger(AnimationString.jumpTrigger);
        rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started && dashEnabled)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        rb.velocity = new Vector2(transform.localScale.x * dashSpeed, rb.velocity.y); // Keep y velocity (for mid-air dashes)
        rb.gravityScale = 0f; // Disable gravity during dash
        trailRenderer.emitting = true;

        yield return new WaitForSeconds(dashDuration); // Wait for dash duration

        rb.gravityScale = baseGravity; // Restore gravity after dash
        trailRenderer.emitting = false; // Stop trail effect
        dashEnabled = false;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        dashEnabled = true;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (!touchingDirections.isGrounded && !CanAirAttack)
            {
                animator.SetBool(AnimationString.usedAirAttack, true);
                animator.SetTrigger(AnimationString.attackTrigger);
            }
            if (touchingDirections.isGrounded)
            {
                animator.SetTrigger(AnimationString.attackTrigger);
            }
        }
    }
    public void OnRangedAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            animator.SetTrigger(AnimationString.rangedAttackTrigger);
        }
    }


    public void OnHit(int damage, Vector2 knockback)
    {
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }
}
