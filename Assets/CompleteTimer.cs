using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteTimer : MonoBehaviour
{
    private bool callOnce = false;
    public TimerSFX TimerSFX;
    public List<DoorOpening> doorsToOpen;
    public List<DoorOpening> doorsToClose;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !callOnce && (TimerSFX == null || TimerSFX.audioSource.isPlaying))
        {
            if (TimerSFX != null)
            {
                TimerSFX.CompleteTimer();
            }
            
            callOnce = true;

            foreach (DoorOpening door in doorsToOpen)
            {
                door.SaveDoor(true);
                door.OpenDoor();
            }
            foreach (DoorOpening door in doorsToClose)
            {
                door.CloseDoor();
            }
        }
    }
}
