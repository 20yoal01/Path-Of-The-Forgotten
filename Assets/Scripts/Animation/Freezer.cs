using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freezer : MonoBehaviour
{
    [Range(0f, 1.5f)]
    public static Freezer Instance;
    public float duration = 0.5f;

    bool _isFrozen = false;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Update()
    {
        if (_pendingFreezeDuration > 0 && !_isFrozen)
        {
            StartCoroutine(DoFreeze());
        }
    }

    float _pendingFreezeDuration = 0f;

    public void Freeze()
    {
        _pendingFreezeDuration = duration;
    }

    IEnumerator DoFreeze()
    {
        _isFrozen = true;
        var original = Time.timeScale;
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = original;
        _pendingFreezeDuration = 0;
        _isFrozen = false;
    }
}
