using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KeyUnlockDoor : MonoBehaviour
{
    public DoorOpening doorToOpen;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!doorToOpen.doorOpen && GameManager.Instance.keyPickedUp)
            {
                doorToOpen.OpenDoor();
            }
        }
    }
}
