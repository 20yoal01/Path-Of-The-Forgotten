using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    public Transform launchPoint;
    public GameObject projectilePrefab;

    private Queue<Vector3> positionHistory = new Queue<Vector3>(); // Track player's position history
    private int maxFrames = 60;
    public Transform playerTransform; // Reference to the player object

    void FixedUpdate()
    {
        TrackPlayerPosition();
    }

    // Track the player's position every frame
    private void TrackPlayerPosition()
    {
        if (positionHistory.Count >= maxFrames)
        {
            positionHistory.Dequeue(); // Remove the oldest position
        }
        positionHistory.Enqueue(GameObject.FindGameObjectWithTag("Player").transform.position); // Add the current position
    }

    // Get the position from 10 frames ago
    private Vector3 GetPositionFromFramesAgo(int framesAgo)
    {
        if (positionHistory.Count < framesAgo)
        {
            // If fewer than 10 frames have passed, return the current position
            return GameObject.FindGameObjectWithTag("Player").transform.position;
        }
        return positionHistory.ToArray()[positionHistory.Count - framesAgo];
    }

    public void FireProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, launchPoint.position, projectilePrefab.transform.rotation);
        Vector3 origScale = projectile.transform.localScale;

        projectile.transform.localScale = new Vector3(
            origScale.x * transform.localScale.x > 0 ? 1 : -1,
            origScale.y,
            origScale.z
            );
    }

    public void FireEnemyProjectile()
    {
        Vector3 targetPosition = GetPositionFromFramesAgo(10);
        GameObject projectileInstance = Instantiate(projectilePrefab, launchPoint.position, projectilePrefab.transform.rotation);
        Rigidbody2D rb = projectileInstance.GetComponent<Rigidbody2D>();


        //rb.velocity = new Vector2(0, 0);

        Vector3 origScale = projectileInstance.transform.localScale;

        projectileInstance.transform.localScale = new Vector3(
            origScale.x * transform.localScale.x > 0 ? 1 : -1,
            origScale.y,
            origScale.z
            );



        Projectile projectileScript = projectileInstance.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.InitializeProjectile(targetPosition); // Pass position before Start runs
        }
        StartCoroutine(FireEnemyProjectileCoroutine(rb));
    }

    private IEnumerator FireEnemyProjectileCoroutine(Rigidbody2D rb)
    {
        Vector2 projectileVelocity = Vector2.zero;
        // Wait for 10 frames before moving the projectile
        for (int i = 0; i < 15; i++)
        {
            if (i == 1)
            {
                if (rb != null)
                {
                    projectileVelocity = rb.velocity;
                    rb.velocity = new Vector2(0, 0);
                }

            }
            
            yield return null; // Wait for the next frame
        }

        if (rb != null)
        {
            // Assuming you're launching it in the right direction; adjust the velocity as needed
            rb.velocity = projectileVelocity;
        }
    }
}
