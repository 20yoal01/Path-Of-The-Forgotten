using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioSource introSource, loopSource;
    public static MusicPlayer Instance { get; private set; }
    public AudioLowPassFilter audioLowPassFilter;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        introSource.Play();
        loopSource.PlayScheduled(AudioSettings.dspTime + introSource.clip.length);
        audioLowPassFilter = GetComponent<AudioLowPassFilter>();
    }

    public void RestartMusic()
    {
        loopSource.Stop();
        introSource.Stop();

        introSource.Play();
        loopSource.PlayScheduled(AudioSettings.dspTime + introSource.clip.length);
        audioLowPassFilter = GetComponent<AudioLowPassFilter>();
    }

    public void toggleAudioSource(bool pause)
    {
        if (pause)
        {
            introSource.Pause();
            loopSource.Pause();
        }
        else
        {
            //introSource.Play();
            loopSource.Play();
        }
    }

    // Update is called once per frame
    public void controlMuffle(float muffle)
    {
        if (audioLowPassFilter != null)
        {
            audioLowPassFilter.cutoffFrequency = muffle;
        }
    }
}
