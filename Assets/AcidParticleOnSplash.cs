using System;
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

            // Calculate the position of the particle in world space
            Vector3 worldPosition = ps.transform.TransformPoint(p.position);

            // Calculate the radius for the particle (using its current size)
            float particleRadius = p.startSize * p.size * 0.5f;

            // Create a temporary GameObject to hold the Collider2D
            GameObject colliderObject = new GameObject($"ParticleCollider_{i}");
            CircleCollider2D circleCollider = colliderObject.AddComponent<CircleCollider2D>();

            // Set the collider's position and size
            circleCollider.transform.position = worldPosition;
            circleCollider.radius = particleRadius;

            // Send the collider to your BubblePop method
            waterObject.GetComponent<WaterTriggerHandler>().BubblePop(circleCollider);

            // Optionally, destroy the collider object if you don't need it anymore
            Destroy(colliderObject);

            // Log the particle information for debugging
            Debug.Log($"Particle {i} collider at {worldPosition} with radius {particleRadius}");
        }
    }
}
