using System.Collections.Generic;
using UnityEngine;

public class MeteoriteShower : MonoBehaviour
{
    ParticleSystem ps;
    Damageable playerDamageable;
    Collider2D playerCol;

    public AudioSource fireHold;
    public AudioSource fireExplosion;
    public AudioSource endingFireHold;

    public int numberOfSounds = 4;
    public float soundVolume = 0.25f;
    private AudioSource[] audioSources;

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
        GameObject playerGM = GameObject.FindGameObjectWithTag("Player");
        if (playerGM != null && ps != null)
        {
            playerGM.TryGetComponent(out playerDamageable);
            playerGM.TryGetComponent(out playerCol);
            ps.trigger.SetCollider(0, playerCol);
        }   
    }

    private void Awake()
    {
        audioSources = new AudioSource[numberOfSounds];

        for (int i = 0; i < numberOfSounds; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = fireHold.clip;
            source.volume = soundVolume;
            source.loop = true; 
            source.playOnAwake = false;

            audioSources[i] = source;
        }

        PlayStackedSounds();
    }

    public void PlayStackedSounds()
    {
        foreach (AudioSource source in audioSources)
        {
            source.Play();
        }
    }

    public void PlayMeteoriteEndingSound()
    {
        endingFireHold.Play();
    }

    public void StopStackedSounds()
    {
        foreach (AudioSource source in audioSources)
        {
            source.Stop();
        }
    }

    private void OnParticleTrigger()
    {
        if (playerDamageable != null)
        {
            playerDamageable.Hit(10, Vector2.zero, false);
            if (fireExplosion != null)
                fireExplosion.Play();
        }
            
    }
}
