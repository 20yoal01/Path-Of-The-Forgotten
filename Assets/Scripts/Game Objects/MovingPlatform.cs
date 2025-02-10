using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float moveSpeed = 2f;

    private Vector3 nextPosition;
    private Vector3 previousPosition;
    private bool activateOnCollision = false;
    private Rigidbody2D rb;
    private bool movingUp;

    public GameObject AttachedLever;
    private LeverTrigger leverTrigger;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        nextPosition = pointB.position;
        previousPosition = transform.position; // Initialize the previous position

        if (AttachedLever != null)
        {
            leverTrigger = AttachedLever.gameObject.GetComponent<LeverTrigger>();
            leverTrigger.InitializePlatform(TogglePlatform);
        }

        if (pointA == null)
        {
            pointA = pointB;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!activateOnCollision || Mathf.Approximately(transform.position.y, nextPosition.y))
            return;

        // Move only in the y-axis
        Vector3 targetPosition = new Vector3(transform.position.x, nextPosition.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (transform.position.y > previousPosition.y)
        {
            movingUp = true;
        }
        else if (transform.position.y < previousPosition.y)
        {
            movingUp = false;
        }

        previousPosition = transform.position; // Update previous position

        // Stop moving when the platform reaches the target position
        if (Mathf.Approximately(transform.position.y, nextPosition.y))
        {
            activateOnCollision = false;
            if (AttachedLever == null)
            {
                TogglePlatform();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.SetCurrentPlatform(this);

            }
            collision.gameObject.transform.parent = transform;
            player.transform.SetParent(collision.transform);

            activateOnCollision = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ClearCurrentPlatform();
            }
            
            if (player.previousParent != null && gameObject.activeInHierarchy)
            {
                collision.gameObject.transform.parent = null;
                player.transform.SetParent(player.previousParent);
            }

            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(ResetParentAfterDelay(collision.gameObject, player));
            }
            

        }
    }

    private IEnumerator ResetParentAfterDelay(GameObject playerObject, PlayerController player)
    {
        yield return null; // Wait for the next frame

        playerObject.transform.parent = null;
        if (player != null && player.previousParent != null)
        {
            playerObject.transform.SetParent(player.previousParent);
        }
    }

    // Toggles the target position of the platform
    public void TogglePlatform()
    {
        nextPosition = (nextPosition == pointA.position) ? pointB.position : pointA.position;
        activateOnCollision = true; // Enable movement
    }

    public float GetAdjustedJumpForce(float baseJumpForce)
    {
        if (!activateOnCollision || Mathf.Approximately(transform.position.y, nextPosition.y))
            return baseJumpForce; // Return the original jump force if the platform is not moving

        float jumpForce;
        if (movingUp)
        {
            jumpForce = baseJumpForce + moveSpeed;
        }
        else
        {
            jumpForce = Mathf.Max(baseJumpForce - moveSpeed, baseJumpForce * 0.5f); // Ensure a minimum jump force
        }
        return jumpForce;
    }
}
