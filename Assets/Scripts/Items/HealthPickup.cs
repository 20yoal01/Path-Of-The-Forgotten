using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healthRestore = 20;
    public Vector3 spinRotationSpeed = new Vector3 (0, 180, 0);

    AudioSource pickupSource;

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles += spinRotationSpeed * Time.deltaTime;
        pickupSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable)
        {
            bool wasHealed = damageable.Heal(healthRestore);

            if (wasHealed)
            {
                if (pickupSource)
                {
                    AudioSource.PlayClipAtPoint(pickupSource.clip, gameObject.transform.position, pickupSource.volume);
                }
                Destroy(gameObject);
            }
        }
    }
}
