using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int attackDamage = 50;
    public Vector2 knockback = Vector2.zero;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // See if it can be hit
        Damageable damageable = collision.GetComponent<Damageable>();
        if (damageable != null)
        {
            Vector2 deliveredKnockback = transform.parent.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);

            bool gotHit = damageable.Hit(attackDamage, deliveredKnockback, true);
            if (gotHit) 
                Debug.Log(collision.name + "hit for " + attackDamage);

            if (gameObject.transform.parent.tag == "Player")
            {
                CinemachineImpulseSource source = GameObject.FindGameObjectWithTag("Player").GetComponent<CinemachineImpulseSource>();
                source.m_DefaultVelocity = new Vector3(knockback.x / 10 + 0.05f, knockback.y / 10, 0 + 0.05f);
                CameraShakeManager.instance.CameraShake(source);
            }
        }
    }
}
