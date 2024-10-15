using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingDemon : MonoBehaviour
{
    public float flightSpeed;
    public float lineOfSight;
    public float shootingRange;

    private Rigidbody2D rb;
    Animator animator;
    private Transform player;
    Damageable damageable;

    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationString.canMove);
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
    }
    private void FixedUpdate()
    {
        if (damageable.IsAlive)
        {
            if (CanMove)
            {
                Act();
            }
            else
            {
                rb.velocity = Vector3.zero;
            }
        }


    }

    private void Act()
    {
        float distanceFromPlayer = Vector2.Distance(player.position, transform.position);

        if (distanceFromPlayer < lineOfSight && distanceFromPlayer > shootingRange)
        {

            transform.position = Vector2.MoveTowards(this.transform.position, player.position, flightSpeed * Time.deltaTime);
            animator.SetBool(AnimationString.isMoving, true);
            animator.SetBool(AnimationString.hasTarget, false);
        }
        else if (distanceFromPlayer <= shootingRange)
        {
            animator.SetBool(AnimationString.hasTarget, true);
            animator.SetBool(AnimationString.isMoving, false);
        }
        else
        {
            animator.SetBool(AnimationString.hasTarget, false);
            animator.SetBool(AnimationString.isMoving, false);
        }

        UpdateDirection();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, lineOfSight);
        Gizmos.DrawWireSphere(transform.position, shootingRange);
    }
    private void UpdateDirection()
    {
        Vector3 locScale = transform.localScale;

        // Check if the player is to the left or right of the enemy
        if (player.position.x < transform.position.x && locScale.x > 0)
        {
            // Player is to the left, flip to face left
            transform.localScale = new Vector3(-locScale.x, locScale.y, locScale.z);
        }
        else if (player.position.x > transform.position.x && locScale.x < 0)
        {
            // Player is to the right, flip to face right
            transform.localScale = new Vector3(-locScale.x, locScale.y, locScale.z);
        }
    }
}
