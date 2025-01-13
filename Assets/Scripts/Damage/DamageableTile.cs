using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableTile : MonoBehaviour
{
    public int damage = 35;
    public float launchDistance = 15;
    public bool returnLastSafePosition;

    public List<Transform> safePositions;
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

                if (collision.gameObject.tag == "Player" && returnLastSafePosition)
                    returnPlayerToSafePosition(collision.gameObject);
            }
            
        }
    }

    private void returnPlayerToSafePosition(GameObject player)
    {
        if (!player.GetComponent<Damageable>().IsAlive)
        {
            return;
        }

        StartCoroutine(FadeOutThenReposition(player));
    }
    private IEnumerator FadeOutThenReposition(GameObject player)
    {
        InputManager.Instance.DeactivatePlayerControls();
        SceneFadeManager.instance.StartFadeOut();
        while (SceneFadeManager.instance.IsFadingOut)
        {
            yield return null;
        }

        //Send player to last position
        Transform closest = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Transform position in safePositions)
        {
            if (position == null) continue;
            float distance = Vector3.Distance(player.transform.position, position.transform.position);
            if (distance * 4 < shortestDistance)
            {
                shortestDistance = distance;
                closest = position;
            }
        }

        player.transform.position = closest.position;
        InputManager.Instance.ActivatePlayerControls();
        SceneFadeManager.instance.StartFadeIn();
    }
}
