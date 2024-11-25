using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchingDirections : MonoBehaviour
{
    public ContactFilter2D castFilter;
    public float groundDistance = 0.05f;
    public float wallDistance = 0.2f;
    public float ceilingDistance = 0.05f;

    CapsuleCollider2D touchingCol;
    Animator animator;

    RaycastHit2D[] groundHits = new RaycastHit2D[5];
    RaycastHit2D[] wallHits = new RaycastHit2D[5];
    RaycastHit2D[] ceilingHits = new RaycastHit2D[5];

    [SerializeField]
    private bool _isGrounded = true;

    public bool isGrounded { get
        {
            return _isGrounded;
        }private set
        {
            _isGrounded = value;
            animator.SetBool(AnimationString.isGrounded, value);
        }
    }

    [SerializeField]
    private bool _isOnWall = true;

    public bool isOnWall
    {
        get
        {
            return _isOnWall;
        }
        private set
        {
            _isOnWall = value;
            animator.SetBool(AnimationString.isOnWall, value);
        }
    }

    [SerializeField]
    private bool _isOnCeiling = true;

    private Vector2 wallCheckDirection => gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;

    public bool isOnCeiling
    {
        get
        {
            return _isOnCeiling;
        }
        private set
        {
            _isOnCeiling = value;
            animator.SetBool(AnimationString.isOnCeiling, value);
        }
    }

    private void Awake()
    {
        touchingCol = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
       isGrounded = touchingCol.Cast(Vector2.down, castFilter, groundHits, groundDistance) > 0;
       isOnCeiling = touchingCol.Cast(Vector2.up, castFilter, ceilingHits, ceilingDistance) > 0;

       int isOnWallNum = touchingCol.Cast(wallCheckDirection, castFilter, wallHits, wallDistance);
       bool damageableTerrain = false;
       for (int i = 0; i < isOnWallNum; i++)
       {
            // Check if the object has the DamageableTile script
           if (wallHits[i].collider.gameObject.GetComponent<DamageableTile>())
           {
                // If no DamageableTile, it's a valid wall for wall jumping
               damageableTerrain = true;
               break; // Stop checking once a valid wall is found
           }
       }

        if (isOnWallNum > 0 && !damageableTerrain)
        {
            isOnWall = true;
        }
        else
        {
            isOnWall = false;
        }
    }
}
