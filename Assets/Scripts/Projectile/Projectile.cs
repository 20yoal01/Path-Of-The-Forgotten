using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 10;
    public Vector2 moveSpeed = new Vector2(10f, 0);
    public Vector2 knockback = new Vector2(0, 0);

    GameObject player;
    public bool targetPlayer;

    Rigidbody2D rb;

    private Vector3 targetPosition; // Target position from 10 frames ago
    private bool isInitialized = false; // Check if the projectile has been initialized

    // Start is called before the first frame update
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (!isInitialized)
        {
            rb.velocity = new Vector2(moveSpeed.x * transform.localScale.x, moveSpeed.y);
        }
    }

    public void InitializeProjectile(Vector3 targetPos)
    {
        targetPosition = targetPos;
        isInitialized = true; // Mark that the projectile has received the target position

        // Launch the projectile now that we have the target position
        LaunchProjectile();
    }

    // This will handle the logic of moving towards the target position
    public void LaunchProjectile()
    {
        if (isInitialized)
        {

            Vector2 moveDir = ((targetPosition - transform.position).normalized * moveSpeed.x);

            // Determine if the enemy is facing right or left (assuming IsFacingRight is a boolean that tracks this)
            bool isFacingRight = transform.localScale.x > 0; // Example check based on scale

            // Calculate the angle differently based on the facing direction
            float angle;
            if (isFacingRight)
            {
                angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg; // Normal angle calculation
            }
            else
            {
                angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg + 180f; // Adjust for left-facing direction
            }

            // Rotate the enemy to face the player
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            // Set the bullet velocity
            rb.velocity = new Vector2(moveDir.x, moveDir.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();


        if (damageable != null)
        {
            Vector2 deliveredKnockback = transform.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);

            bool gotHit = damageable.Hit(damage, deliveredKnockback, true);
            if (gotHit)
                Debug.Log(collision.name + "hot for " + damage);
        }
        Destroy(gameObject);
    }
}
