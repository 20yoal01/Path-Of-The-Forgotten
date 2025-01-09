using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PlayerOneShotBehaviour : StateMachineBehaviour
{
    public AudioClip soundToPlay;
    public float volume = 0.25f;
    public bool playOnEnter = true, playOnExit = false, playAfterDelay = false;

    // Delayed sound timer
    public float playDelay = 0.25f;
    private float timeSinceEntered = 0;
    private bool hasDelayedSoundPlayed = false;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playOnEnter)
        {
            playAudioPitchShift(animator);
        }

        timeSinceEntered = 0f;
        hasDelayedSoundPlayed = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playAfterDelay && !hasDelayedSoundPlayed)
        {
            timeSinceEntered += Time.deltaTime;

            if (timeSinceEntered > playDelay)
            {
                playAudioPitchShift(animator);
                hasDelayedSoundPlayed = true;
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playOnExit)
        {
            playAudioPitchShift(animator);
        }
    }

    private void playAudioPitchShift(Animator animator)
    {
        float pitch = Random.Range(0.9f, 1.1f);

        // Create a temporary GameObject at the specified position
        GameObject tempAudioSource = new GameObject("TempAudioSource");
        tempAudioSource.transform.position = animator.gameObject.transform.position;

        // Add an AudioSource component
        AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
        audioSource.clip = soundToPlay;
        audioSource.volume = this.volume;
        audioSource.pitch = pitch;

        audioSource.Play();

        // Destroy the GameObject after the clip finishes playing
        Destroy(tempAudioSource, soundToPlay.length / Mathf.Abs(audioSource.pitch));
    }
}
