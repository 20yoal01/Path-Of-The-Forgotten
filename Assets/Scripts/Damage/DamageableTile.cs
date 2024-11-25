using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableTile : MonoBehaviour
{
    public int damage = 35;
    public float launchDistance = 15;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Damageable damageable = collision.gameObject.GetComponent<Damageable>();
        if (damageable)
        {
            Rigidbody2D playerRigidbody = collision.gameObject.GetComponent<Rigidbody2D>();

            if (playerRigidbody != null)
            {
                // Get the contact point and normal direction of the collision
                ContactPoint2D contact = collision.GetContact(0);
                Vector2 knockbackDirection = -contact.normal; // Use the collision normal for knockback direction

                // Apply damage and knockback to the player
                damageable.Hit(damage, knockbackDirection * launchDistance, false);
            }
            
        }
    }
}
