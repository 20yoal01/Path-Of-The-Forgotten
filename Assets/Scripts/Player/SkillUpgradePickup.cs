using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUpgradePickup : MonoBehaviour
{
    public Ability ability;
    private AbilityManager abilityManager;
    AudioSource pickupSource;
    private void Start()
    {
        // Access the singleton instance directly
        abilityManager = AbilityManager.Instance;
    }

    private void Update()
    {
        pickupSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            abilityManager.UnlockSkill(ability);
            AudioSource.PlayClipAtPoint(pickupSource.clip, gameObject.transform.position, pickupSource.volume);
            CharacterEvents.characterAbilityUnlock.Invoke(gameObject, ability);
            InputManager.Instance.SaveAsync();
            Destroy(gameObject);
        }
    }
}
