using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableTile : MonoBehaviour
{
    public int damage = 35;
    public float launchDistance = 15;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();
        if (damageable)
        {
            Rigidbody2D playerRigidbody = collision.GetComponent<Rigidbody2D>();

            if (playerRigidbody != null)
            {
                Vector2 playerVelocity = playerRigidbody.velocity;
                Vector2 knockbackDirection = -playerVelocity.normalized;
                damageable.Hit(damage, knockbackDirection * launchDistance, false);
            }
            
        }
    }
}
