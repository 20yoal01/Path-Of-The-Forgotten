using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayEffect(GameObject effectPrefab, Vector2 position, float direction = 1, Quaternion? rotation = null)
    {
        if (effectPrefab == null)
        {
            Debug.LogWarning("Effect prefab is null. Cannot play effect.");
            return;
        }

        var finalRotation = rotation ?? (direction >= 0 ? Quaternion.identity : Quaternion.Euler(0, 180, 0));

        var effectInstance = Instantiate(effectPrefab, position, finalRotation);

        var effectDuration = GetEffectDuration(effectInstance);
        Destroy(effectInstance, effectDuration);
    }

    private float GetEffectDuration(GameObject effect)
    {
        var animator = effect.GetComponent<Animator>();
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            return animator.GetCurrentAnimatorStateInfo(0).length;
        }

        var particleSystem = effect.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            return particleSystem.main.duration + particleSystem.main.startLifetime.constantMax;
        }

        return 2.0f;
    }
}
