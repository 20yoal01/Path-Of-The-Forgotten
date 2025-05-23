using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class PlayerController : MonoBehaviour
{
    public ParticleSystem dust;

    public Transform previousParent;

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
    private float jumpLoss = 0.3f;


    private float _fallSpeedYDampingChangeThreshold;

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
    public AudioSource dashSF;


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

    public bool IsOnWall
    {
        get
        {
            if (!canWallClimb)
            {
                return false;
            }
            bool shouldBeOnWall = touchingDirections.isOnWall && canWallClimb;
            animator.SetBool(AnimationString.isOnWall, shouldBeOnWall);
            return shouldBeOnWall;
        }
    }

    Rigidbody2D rb;
    Animator animator;

    [Header("Player Abilities")]
    public bool canArrowBarrage = false;
    public bool canShootBow = false;
    public bool canArrowHeal = false;
    public bool canDoubleJump = false;
    public bool canWallClimb = false;
    public bool canDash = false;
    public bool increasedHP = false;
    public int _arrowsRemaining = 0;
    public int maxArrows = 8;
    public UnityEvent<int> ammoRemaningEvent = new UnityEvent<int>();

    public UnityEvent<bool> healthUpgradedEvent = new UnityEvent<bool>();

    public int ArrowsRemaining { get { 
            return _arrowsRemaining; } set
        {
            if (_arrowsRemaining != value) // Check to avoid unnecessary invocations
            {
                _arrowsRemaining = value;
                ammoRemaningEvent?.Invoke(_arrowsRemaining);
            }
        }
    }


    public void OnEnableAbility(Ability ability)
    {
        switch (ability)
        {
            case Ability.Bow:
                canShootBow = true;
                break;
            case Ability.DoubleJump:
                canDoubleJump = true;
                break;
            case Ability.WallClimb:
                canWallClimb = true;
                break;
            case Ability.ArrowBarrage:
                canArrowBarrage = true;
                break;
            case Ability.Dash:
                canDash = true;
                break;
            case Ability.HealthIncrease:
                damageable.MaxHealth = 125;
                damageable.Health = damageable.MaxHealth;
                if (!increasedHP)
                    healthUpgradedEvent?.Invoke(true);
                increasedHP = true;

                break;
            case Ability.ArrowHeal:
                canArrowHeal = true;
                break;
        }
    }
    private void Start()
    {
        _fallSpeedYDampingChangeThreshold = CameraManager.instance._fallSpeedYDampingChangeThreshhold;
        previousParent = transform.parent;
    }

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
        if (rb.linearVelocity.y < _fallSpeedYDampingChangeThreshold && !CameraManager.instance.IsLerpingYDamping && !CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpYDaming(true);
        }

        if (rb.linearVelocity.y >= 0f && !CameraManager.instance.IsLerpingYDamping && CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpedFromPlayerFalling = false;
            CameraManager.instance.LerpYDaming(false);
        }

        if (IsOnWall && !touchingDirections.isGrounded && rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector2(0, Mathf.Clamp(rb.linearVelocity.y, -wallSlideSpeed, float.MaxValue));
        }
        else
        {
            Gravity();
        }


        float velocityX;
        if (wallJumping || isDashing || isHurt)
        {
            velocityX = rb.linearVelocity.x;
        }
        else
        {
            velocityX = moveInput.x * CurrentMoveSpeed;
        }

        rb.linearVelocity = new Vector2(velocityX, rb.linearVelocity.y);
            
        animator.SetFloat(AnimationString.yVelocity, rb.linearVelocity.y);
        if (touchingDirections.isGrounded || IsOnWall)
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
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpLoss);
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
        bool oldFacingDir = IsFacingRight;

        if (moveInput.x > 0 && !IsFacingRight)
        {
            IsFacingRight = true;
        }
        else if(moveInput.x < 0 && IsFacingRight)
        {
            IsFacingRight = false;
        }

        if (oldFacingDir != IsFacingRight && touchingDirections.isGrounded)
        {
            createDust();
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("player_bow") || animator.GetCurrentAnimatorStateInfo(0).IsName("player_bow_up"))
        {
            bool bowUp = animator.GetBool("bowUp");
            CameraManager.instance.AdjustCameraForBowCharge(bowUp);
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

        if(rb.linearVelocity.y <= 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMultiplier;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
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
        if (context.performed && IsOnWall && CanMove && !touchingDirections.isGrounded)
        {
            WallJump();
        }
        if (context.started && coyoteTimeCounter > 0 && CanMove)
        {
            createDust();
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
            && !IsOnWall
            && doubleJumpAvailable 
            && !usedJumpWhileAir && CanMove
            && canDoubleJump
            )
        {
            usedJumpWhileAir = true;
            doubleJumpAvailable = false;
            PerformJump();
            jumpCounter = 0f;
            isJumping = true;
        }
        else if (context.canceled && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpLoss);
            isJumping = false;
            coyoteTimeCounter = 0f;
        }
    }

    private void WallJump()
    {
        IsFacingRight = !IsFacingRight;
        animator.SetTrigger(AnimationString.jumpTrigger);
        rb.linearVelocity = new Vector2(wallJumpForce.x * (IsFacingRight ? 1 : -1), jumpImpulse); // Apply horizontal and vertical force
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
        float adjustedJumpForce = calculateJumpForce();


        dashEnabled = true;
        animator.SetTrigger(AnimationString.jumpTrigger);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, adjustedJumpForce);
    }

    private float calculateJumpForce()
    {
        float adjustedJumpForce = 0f;
        if (IsOnPlatform())
        {
            MovingPlatform platform = currentPlatform.GetComponent<MovingPlatform>();
            adjustedJumpForce = platform.GetAdjustedJumpForce(jumpImpulse);
        }
        else
        {
            adjustedJumpForce = jumpImpulse;
        }

        return adjustedJumpForce;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (!canDash)
            return;

        if (context.started && dashEnabled)
        {
            dashSF.Play();
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        rb.linearVelocity = new Vector2(transform.localScale.x * dashSpeed, 0); // Keep y velocity (for mid-air dashes)
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

    private bool isWindingUp = false;
    private bool isShooting = false;


    public void AddArrowsByOne()
    {
        if (_arrowsRemaining >= maxArrows)
            return;
        ArrowsRemaining++;
    }

    public void AdjustBowCamera(float diagonalFloat)
    {
        bool diagonal = diagonalFloat >= 1;
        CameraManager.instance.AdjustCameraForBowCharge(diagonal);
    }

    public void ResetCamera()
    {
        CameraManager.instance.ResetCameraOffset();
    }

    private bool arrowBarrageActive = false;
    public void ArrowBarrageAbility(InputAction.CallbackContext context)
    {
        if (!canArrowBarrage || !touchingDirections.isGrounded)
            return;

        if (context.started)
        {
            if (ArrowsRemaining < 5)
            {
                return;
            }

            ArrowsRemaining = ArrowsRemaining - 5;
            animator.SetBool(AnimationString.arrowBarrage, true);
            arrowBarrageActive = true;
            StartCoroutine(ArrowBarrageTimer());
        }
    }

    private int arrowBarrageTime = 3;
    private bool switchActionMap = false;
    private IEnumerator ArrowBarrageTimer()
    {
        yield return new WaitForSeconds(arrowBarrageTime);
        arrowBarrageActive = false;
    }

    public void OnRangedAttackWindUp(InputAction.CallbackContext context)
    {
        if (!canShootBow || isWindingUp || isShooting || !touchingDirections.isGrounded)
        {
            return;
        }

        if (context.started)
        {
            animator.SetFloat("arrowsRemaining", ArrowsRemaining);
            animator.SetTrigger(AnimationString.rangedAttackTrigger);
        }

        if (context.performed)
        {
            if (!arrowBarrageActive)
            {
                animator.SetBool(AnimationString.arrowBarrage, false);
                if (ArrowsRemaining > 0)
                    ArrowsRemaining--;
                isWindingUp = true; // Mark as winding up
                animator.SetBool(AnimationString.bowUp, false);
                InputManager.Instance.SwitchToBowActionMap();
                switchActionMap = true;
            }

            
        }
    }

    private IEnumerator ResetBowStateAfterAnimation()
    {
        animator.SetTrigger(AnimationString.shootRangedAttackTrigger);

        while (animator.GetBool("shootRangedAttack"))
        {
            yield return null; // Wait for the next frame
        }

        isShooting = false; // Reset shooting state
        ammoRemaningEvent?.Invoke(ArrowsRemaining);
        InputManager.Instance.SwitchToPlayerActionMap();
    }

    public void OnRangedAttackRelease(InputAction.CallbackContext context)
    {
        if (!canShootBow || isShooting)
        {
            return;
        }

        if (context.canceled && switchActionMap)
        {
            isWindingUp = false; // Wind-up completed
            isShooting = true; // Mark as shooting

            StartCoroutine(ResetBowStateAfterAnimation());
            switchActionMap = false;
        }
    }

    public void UpdateBowRotation(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 aimInput = context.ReadValue<Vector2>();

            if (aimInput.x == 0 && aimInput.y == 0)
            {
                return;
            }

            bool aimAngle = false;
            if ((aimInput.x == 1 && aimInput.y == 0) || (aimInput.x < 0 && aimInput.y > 0))
                aimAngle = true;

            animator.SetBool(AnimationString.bowUp, aimAngle);

            //animator.SetBool(AnimationString.bowUp, isBowUp);
            SetFacingDirection(aimInput);

        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        Invoke("getHurt", hurtDuration);
        rb.linearVelocity = new Vector2(knockback.x, rb.linearVelocity.y + knockback.y);
        isHurt = true;
        doubleJumpAvailable = false;
        Invoke("getHurt", hurtDuration);
    }

    void getHurt()
    {
        isHurt = false;
    }

    private MovingPlatform currentPlatform;

    public void SetCurrentPlatform(MovingPlatform platform)
    {
        currentPlatform = platform;
    }

    public void ClearCurrentPlatform()
    {
        currentPlatform = null;
    }

    public bool IsOnPlatform()
    {
        return currentPlatform != null;
    }

    private void createDust()
    {
        dust.Play();
    }

    public void Save(ref PlayerSaveData data)
    {
        data.Position = transform.position;
        data.canShootBow = canShootBow;
        data.canDoubleJump = canDoubleJump;
        data.canWallClimb = canWallClimb;
        data.arrowBarrage = canArrowBarrage;
        data.canArrowHeal = canArrowHeal;
        data.increasedHP = increasedHP;
    }

    public void Load(PlayerSaveData data)
    {
        transform.position = data.Position;
        canShootBow = data.canShootBow;
        canDoubleJump = data.canDoubleJump;
        canWallClimb = data.canWallClimb;
        canArrowHeal = data.canArrowHeal;
        canArrowBarrage = data.arrowBarrage;
        increasedHP = data.increasedHP;
        if (damageable != null && increasedHP)
        {
            damageable.MaxHealth = 125;
            damageable.Health = damageable.MaxHealth;
        }
        healthUpgradedEvent?.Invoke(increasedHP);
    }
}


[System.Serializable]
public struct PlayerSaveData
{
    public Vector3 Position;
    public bool canShootBow;
    public bool canDoubleJump;
    public bool canWallClimb;
    public bool canArrowHeal;
    public bool arrowBarrage;
    public bool increasedHP;
}