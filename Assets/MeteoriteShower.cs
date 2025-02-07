using System.Collections.Generic;
using UnityEngine;

public class MeteoriteShower : MonoBehaviour
{
    ParticleSystem ps;
    Damageable playerDamageable;
    Collider2D playerCol;

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
        GameObject playerGM = GameObject.FindGameObjectWithTag("Player");
        if (playerGM != null && ps != null)
        {
            playerGM.TryGetComponent(out playerDamageable);
            playerGM.TryGetComponent(out playerCol);
            ps.trigger.SetCollider(0, playerCol);
        }   
    }

    private void OnParticleTrigger()
    {
        if (playerDamageable != null)
            playerDamageable.Hit(10, Vector2.zero, false);
    }
}
