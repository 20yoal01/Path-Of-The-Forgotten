using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    public Transform launchPoint;

    private Queue<Vector3> positionHistory = new Queue<Vector3>(); // Track player's position history
    private int maxFrames = 5;
    public Transform playerTransform; // Reference to the player object


    public List<ProjectileEntry> projectileEntries; // List of projectile types

    private Dictionary<string, GameObject> projectileDictionary;

    [System.Serializable]
    public class ProjectileEntry
    {
        public string projectileName;
        public GameObject projectilePrefab;
    }

    private void Awake()
    {
        projectileDictionary = new Dictionary<string, GameObject>();
        foreach (var entry in projectileEntries)
        {
            if (!projectileDictionary.ContainsKey(entry.projectileName))
            {
                projectileDictionary.Add(entry.projectileName, entry.projectilePrefab);
            }
        }
    }

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

    public void FireProjectile(string projectileNameAndAngle)
    {
        string[] parts = projectileNameAndAngle.Split(',');

        string projectileName = parts[0];
        float angle = float.Parse(parts[1]);

        Animator animator = GameObject.FindWithTag("Player").GetComponent<Animator>();
        float arrowsRemaining = animator.GetFloat("arrowsRemaining");

        if (projectileName == "Charged Arrow" && arrowsRemaining == 0)
        {
            projectileName = "Weak Arrow";
        }

        if (!projectileDictionary.ContainsKey(projectileName))
        {
            Debug.LogWarning($"Projectile type '{projectileName}' not found!");
            return;
        }

        GameObject projectilePrefab = projectileDictionary[projectileName];

        GameObject projectile = Instantiate(projectilePrefab, launchPoint.position, projectilePrefab.transform.rotation);
        Vector3 origScale = projectile.transform.localScale;
        bool isFacingRight = transform.localScale.x > 0;
        projectile.transform.localScale = new Vector3(
            origScale.x * (isFacingRight ? 1 : -1),
            origScale.y,
            origScale.z
        );

        float radianAngle = angle * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle));
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction;
            if (!isFacingRight)
            {
                rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
            }
        }
        float rotationAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (!isFacingRight)
        {
            rotationAngle = -rotationAngle;
            if (projectileName != "Barrage Arrow")
                rotationAngle += 135;
        }
        else
        {
            if (projectileName != "Barrage Arrow")
                rotationAngle -= 135;
        }
        projectile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotationAngle));
    }


    public void FireEnemyProjectile()
    {
        string projectileName = "Enemy";

        if (!projectileDictionary.ContainsKey(projectileName))
        {
            Debug.LogWarning($"Projectile type '{projectileName}' not found!");
            return;
        }

        GameObject projectilePrefab = projectileDictionary[projectileName];

        Vector3 targetPosition = GetPositionFromFramesAgo(10);
        GameObject projectileInstance = Instantiate(projectilePrefab, launchPoint.position, projectilePrefab.transform.rotation);
        Rigidbody2D rb = projectileInstance.GetComponent<Rigidbody2D>();


        //rb.velocity = new Vector2(0, 0);

        Vector3 origScale = projectileInstance.transform.localScale;

        projectileInstance.transform.localScale = new Vector3(
            origScale.x * (transform.localScale.x > 0 ? 1 : -1),
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
