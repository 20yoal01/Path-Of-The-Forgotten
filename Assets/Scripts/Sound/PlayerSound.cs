using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    public AudioSource walkFS;
    void PlayFootstep()
    {
        walkFS.volume = Random.Range(0.3f, 0.5f);
        walkFS.pitch = Random.Range(0.8f, 1.2f);
        walkFS.Play();
    }
}
