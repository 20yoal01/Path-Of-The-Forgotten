using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class PlayerController : MonoBehaviour
{   
    //Speed
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float airWalkSpeed = 3f;
    
    
    Vector2 moveInput;
    TouchingDirections touchingDirections;
    Damageable damageable;

    // Jump durations
    public float jumpImpulse = 10f;
    public float jumpTime;
    private bool isJumping;
    private float jumpCounter;
    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    
    // Double Jumps
    public bool doubleJumpAvailable = true;
    public bool usedJumpWhileAir = false;

    //Gravity | FallSpeed
    [Header("Gravity")]
    public float baseGravity = 1f;
    public float maxFallSpeed = 6f;
    public float fallSpeedMultiplier = 1.2f;


    //Dashing
    public float dashDuration = 0.5f;
    public float dashSpeed = 40f;
    private TrailRenderer trailRenderer;
    private bool dashEnabled = true;
    private bool isDashing = false;
    private float dashCooldown = 1f;
    public float distanceBetweenImages;
    private float lastImageXpos;


    //WallJumps
    public float wallSlideSpeed = 2f;
    public float wallJumpDuration = 0.1f;
    public Vector2 wallJumpForce = new Vector2(10, 20);
    bool wallJumping;

    private Vector2 currentMoveInput;
    private bool isHurt = false;
    private float hurtDuration = 0.3f;

    public float CurrentMoveSpeed
    {
        get
        {
            if (!CanMove)
            {
                // Movement is locked
                return 0;
            }

            if (isDashing)
            {
                return dashSpeed;
            }

            if (touchingDirections.isOnWall)
            {
                // Movement behavior when on a wall (if different from air movement, adjust accordingly)
                return 0;
            }

            if (IsMoving)
            {
                if (touchingDirections.isGrounded)
                {
                    return IsRunning ? runSpeed : walkSpeed;
                }
                else
                {
                    return airWalkSpeed;
                }
            }

            // Default to air speed if not moving
            return airWalkSpeed;
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
        if (touchingDirections.isOnWall && !touchingDirections.isGrounded && rb.velocity.y < 0)
        {
            rb.velocity = new Vector2(0, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue));
        }
        else
        {
            Gravity();
        }


        float velocityX;
        if (wallJumping || isDashing || isHurt)
        {
            velocityX = rb.velocity.x;
        }
        else
        {
            velocityX = moveInput.x * CurrentMoveSpeed;
        }

        rb.velocity = new Vector2(velocityX, rb.velocity.y);
            
        animator.SetFloat(AnimationString.yVelocity, rb.velocity.y);
        if (touchingDirections.isGrounded || touchingDirections.isOnWall)
        {
            doubleJumpAvailable = true;
            usedJumpWhileAir = false;
            animator.SetBool(AnimationString.usedAirAttack, false);
        }

        if (touchingDirections.isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (isJumping)
        {
            jumpCounter += Time.fixedDeltaTime;
            if (jumpCounter >= jumpTime)
            {
                isJumping = false; // End jump after jumpTime
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            }
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        currentMoveInput = moveInput;

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
        if (isDashing || wallJumping || isHurt)
        {
            return;
        }

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
        if (context.performed && touchingDirections.isOnWall && CanMove && !touchingDirections.isGrounded)
        {
            WallJump();
        }
        if (context.started && coyoteTimeCounter > 0 && CanMove)
        {
            PerformJump();
            isJumping = true;
            jumpCounter = 0f;
            coyoteTimeCounter = 0f;
            usedJumpWhileAir = false; 
        }
        // Double jump logic (in air)
        else if (context.started 
            && !touchingDirections.isGrounded 
            && !touchingDirections.isOnCeiling 
            && !touchingDirections.isOnWall
            && doubleJumpAvailable 
            && !usedJumpWhileAir && CanMove
            )
        {
            usedJumpWhileAir = true;
            doubleJumpAvailable = false;
            PerformJump();
            jumpCounter = 0f;
            isJumping = true;
        }
        else if (context.canceled && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            isJumping = false;
            coyoteTimeCounter = 0f;
        }
    }

    private void WallJump()
    {
        IsFacingRight = !IsFacingRight;
        rb.velocity = new Vector2(wallJumpForce.x * (IsFacingRight ? 1 : -1), jumpImpulse); // Apply horizontal and vertical force
        wallJumping = true;
        Invoke("StopWallJump", wallJumpDuration);
    }

    void StopWallJump()
    {
        wallJumping = false;
        doubleJumpAvailable = true;
        usedJumpWhileAir = false;
        SetFacingDirection(currentMoveInput);
    }

    private void PerformJump()
    {
        dashEnabled = true;
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
        rb.velocity = new Vector2(transform.localScale.x * dashSpeed, 0); // Keep y velocity (for mid-air dashes)
        rb.gravityScale = 0f; // Disable gravity during dash
        //trailRenderer.emitting = true;
        PlayerAfterImagePool.Instance.GetFromPool();
        lastImageXpos = transform.position.x;

        float dashTimeElapsed = 0f;

        // Create afterimages while dashing
        while (dashTimeElapsed < dashDuration)
        {
            if (Mathf.Abs(transform.position.x - lastImageXpos) > distanceBetweenImages)
            {
                PlayerAfterImagePool.Instance.GetFromPool();
                lastImageXpos = transform.position.x;
            }

            dashTimeElapsed += Time.deltaTime;
            yield return null; // Wait for next frame
        }

        rb.gravityScale = baseGravity; // Restore gravity after dash
        //trailRenderer.emitting = false; // Stop trail effect
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
        Invoke("getHurt", hurtDuration);
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
        isHurt = true;
        doubleJumpAvailable = false;
        Invoke("getHurt", hurtDuration);
    }

    void getHurt()
    {
        isHurt = false;
    }
}
