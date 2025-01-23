using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimerSFX : MonoBehaviour
{
    public AudioSource audioSource;
    public float startPitch = 1.0f;
    public float maxPitch = 1.5f;   // Maximum pitch value (how fast the sound gets)
    private GameObject door;
    private MusicPlayer player;

    private int timerDuration;
    DoorOpening doorOpening;

    private Coroutine pitchOverTimeCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        player = MusicPlayer.Instance;
        doorOpening = gameObject.GetComponent<DoorOpening>();
        timerDuration = doorOpening.doorCloseCoolDown;
        doorOpening.AttachedLever.GetComponent<LeverTrigger>().Initialize(playSound, timerDuration);

        bool doorAlreadyUnlocked = false;
        GameManager.Instance.doorNameOpening.TryGetValue(doorOpening.assignedDoorName, out doorAlreadyUnlocked);

        if (doorAlreadyUnlocked)
        {
            doorOpening.AttachedLever.GetComponent<LeverTrigger>().enabled = false;
            doorOpening.AttachedLever.GetComponent<Collider2D>().enabled = false;
        }
    }

    public void playSound()
    {
        pitchOverTimeCoroutine = StartCoroutine(IncreasePitchOverTime());
    }

    public void CompleteTimer()
    {
        StopCoroutine(pitchOverTimeCoroutine);
        doorOpening.OpenDoor();
        doorOpening.doorCloseCoolDown = 0;
        doorOpening.AttachedLever.GetComponent<LeverTrigger>().enabled = false;
        audioSource.Stop();
        player.toggleAudioSource(false);
    }

    private IEnumerator IncreasePitchOverTime()
    {
        player.toggleAudioSource(true);
        audioSource.loop = true;
        audioSource.Play();
        //player.pauseAudioSource();

        audioSource.pitch = startPitch;  // Get current pitch
        float timeElapsed = 0f;

        // Start the sound effect


        // Gradually increase the pitch of the sound over time
        while (timeElapsed < timerDuration)
        {
            // Increase pitch over time
            audioSource.pitch = Mathf.Lerp(startPitch, maxPitch, timeElapsed / timerDuration);

            timeElapsed += Time.deltaTime;
            yield return null;  // Wait for the next frame
        }

        audioSource.loop = false;
        audioSource.Stop();  // Stop the sound after the timer duration
        player.toggleAudioSource(false);
    }
}
