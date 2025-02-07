using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ItemType {Arrow}

public class ItemPickup : MonoBehaviour
{
    [SerializeField]
    public ItemType itemType;
    AudioSource pickupSource;

    private void Update()
    {
        pickupSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (itemType == ItemType.Arrow)
            {
                PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
                if (playerController.ArrowsRemaining >= playerController.maxArrows)
                {
                    return;
                }
                AudioSource.PlayClipAtPoint(pickupSource.clip, gameObject.transform.position, pickupSource.volume);
                playerController.AddArrowsByOne();
            }
            Destroy(gameObject);
        }
    }
}
