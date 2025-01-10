using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class AcidParticleOnSplash : MonoBehaviour
{
    public GameObject waterObject;
    ParticleSystem ps;

    private void Start()
    {
        EdgeCollider2D waterCol = waterObject.GetComponent<EdgeCollider2D>();

        if (waterCol == null)
            return;

        ps = GetComponent<ParticleSystem>();

        ps.trigger.SetCollider(0, waterCol);

    }

    private void OnParticleTrigger()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();

        // Get particles that entered the trigger
        int numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, enter);

        // Iterate over all triggered particles
        for (int i = 0; i < numEnter; i++)
        {
            ParticleSystem.Particle p = enter[i];

            // Calculate the bounds for the particle
            Vector2 particleCenter = p.position; // Particle's world position
            float particleRadius = p.startSize * 0.5f; // Half of the start size (assuming the particle is roughly circular)

            // Create the bounds (AABB)
            Bounds particleBounds = new Bounds(particleCenter, new Vector3(particleRadius * 2, particleRadius * 2, 0));

            // Use the bounds for collision checking or other logic
            // For example, you can check if the bounds are inside a specific area (like water)
            Debug.Log($"Particle {i} bounds: {particleBounds}");

            // Optionally, you can check if the particle intersects with any colliders (like water)
            Collider2D[] hitColliders = Physics2D.OverlapBoxAll(particleBounds.center, particleBounds.size, 0f);
            foreach (var hitCollider in hitColliders)
            {
                waterObject.GetComponent<WaterTriggerHandler>().BubblePop(hitCollider);
            }
        }
    }
}
