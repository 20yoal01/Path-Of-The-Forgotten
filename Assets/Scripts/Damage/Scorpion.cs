using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Scorpion : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;

    private Transform currentPoint;

    private float currentSpeed = 0f;
    public float speed = 1f;
    public DetectionZone attackZone;

    Rigidbody2D rb;
    TouchingDirections touchingDirections;
    Animator animator;
    Damageable damageable;

    public bool hasTurned;
    [SerializeField]
    private float ZAxisAdd;
    public float fallTime;

    [Header("Ground Detection")]
    public bool groundDetected;
    [SerializeField] Transform groundPos;
    public float groundCheckSize;
    [SerializeField]
    public LayerMask whatIsGround;

    [Header("Wall Detection")]
    public bool wallDetected;
    [SerializeField] Transform wallPos;
    public float wallCheckSize;

    [SerializeField]
    private bool _faceRight = false;
    public bool faceRight
    {
        get
        {
            return _faceRight;
        }
        set
        {
            if (_faceRight != value)
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y);
            _faceRight = value;
        }
    }

    public float getDirectionFloat
    {
        get
        {
            if (faceRight)
            {
                return -1;
            }
            return 1;
        }
    }

    public float AttackCooldown
    {
        get
        {
            return animator.GetFloat(AnimationString.AttackCooldown);
        }
        private set
        {
            animator.SetFloat(AnimationString.AttackCooldown, Mathf.Max(value, 0));
        }
    }

    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationString.canMove);
        }
    }

    public bool _hasTarget = false;
    public bool HasTarget
    {
        get { return _hasTarget; }
        private set
        {
            _hasTarget = value;
            animator.SetBool(AnimationString.hasTarget, value);
        }
    }

    private void Awake()
    {
        currentSpeed = speed;
        faceRight = _faceRight;
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();
        currentPoint = pointA;
    }

    private void Start()
    {
        if (damageable != null)
        {
            damageable.deathEvent.AddListener(GameManager.Instance.ScorpionQuest.OnScorpionKilled);
        }
    }

    private void Update()
    {
        HasTarget = attackZone.DetectedColliders.Count > 0;
        if (!HasTarget)
        {
            animator.SetBool(AnimationString.canMove, true);
        }
        if (AttackCooldown > 0)
        {
            AttackCooldown -= Time.deltaTime;
        }
        Environment();
    }

    private void FixedUpdate()
    {
        if (!damageable.LockVelocity)
        {
            if (CanMove)
                Movement();
            else
            {
                rb.velocity = new Vector2(0, 0);
            }
        }

        if(Vector2.Distance(currentPoint.position, gameObject.transform.position) < 1f)
        {
            currentPoint = currentPoint == pointA ? pointB : pointA;
            faceRight = !faceRight;
        }
    }

    void Movement()
    {
        rb.velocity = -transform.right * speed * getDirectionFloat;
    }

    void Environment()
    {
        groundDetected = Physics2D.Raycast(groundPos.position, -transform.up, groundCheckSize, whatIsGround);
        wallDetected = Physics2D.Raycast(wallPos.position, -transform.right * getDirectionFloat, wallCheckSize, whatIsGround);

        if (!groundDetected)
        {
            if (hasTurned == false)
            {
                ZAxisAdd = ZAxisAdd + (90 * getDirectionFloat);
                transform.eulerAngles = new Vector3(0, 0, ZAxisAdd);
                hasTurned = true;
            }

            fallTime -= Time.deltaTime;
        }

        if (groundDetected)
        {
            hasTurned = false;

            fallTime = 1;
        }

        if (wallDetected)
        {
            if (!hasTurned)
            {
                ZAxisAdd = ZAxisAdd + (-90 * getDirectionFloat);
                transform.eulerAngles = new Vector3(0, 0, ZAxisAdd);
            }
        }

        if (fallTime == 1)
        {
            rb.gravityScale = 0;
            currentSpeed = speed;
        }
        else if (fallTime <= 0)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            ZAxisAdd = 0;
            rb.gravityScale = 50;
            currentSpeed = 0;
        }

        if (ZAxisAdd <= -360)
        {
            ZAxisAdd = 0;
        }
    }

}
