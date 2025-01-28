using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
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
            AudioSource.PlayClipAtPoint(pickupSource.clip, gameObject.transform.position, pickupSource.volume);
            if (itemType == ItemType.Arrow)
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().AddArrowsByOne();
            }
            Destroy(gameObject);
        }
    }
}
