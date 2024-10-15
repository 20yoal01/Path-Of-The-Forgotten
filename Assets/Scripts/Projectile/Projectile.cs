using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 10;
    public Vector2 moveSpeed = new Vector2(10f, 0);
    public Vector2 knockback = new Vector2(0, 0);

    public bool targetPlayer;

    Rigidbody2D rb;
    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Start()
    {
        if (targetPlayer)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Vector2 moveDir = ((player.transform.position - transform.position).normalized * moveSpeed.x);

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
        else
        {
            rb.velocity = new Vector2(moveSpeed.x, moveSpeed.y);
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
