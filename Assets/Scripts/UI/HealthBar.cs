using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public TMP_Text healthBarText;

    Damageable playerDamageable;

    public string TagToAttach;
    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag(TagToAttach);

        if(player == null)
        {
            Debug.Log("No player found in the scene. Make sure it has tag 'Player'");
        }
        else
        {
            playerDamageable = player.GetComponent<Damageable>();
        }

        
    }

    // Start is called before the first frame update
    void Start()
    {
        if (playerDamageable == null)
        {
            return;
        }

        healthSlider.value = (float) playerDamageable.Health / (float) playerDamageable.MaxHealth;
        if (healthBarText != null)
            healthBarText.text = "HP " + playerDamageable.Health + " / " + playerDamageable.MaxHealth;
    }

    private void OnEnable()
    {
        if (playerDamageable != null)
            playerDamageable.healthChanged.AddListener(OnPlayerHealthChanged);
    }

    private void OnDisable()
    {
        if (playerDamageable != null)
            playerDamageable.healthChanged.RemoveListener(OnPlayerHealthChanged);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnPlayerHealthChanged(float newHealth, float maxHealth)
    {
        if (playerDamageable == null)
        {
            return;
        }
        healthSlider.value = newHealth / maxHealth;
        if (healthBarText != null)
            healthBarText.text = "HP " + newHealth + " / " + maxHealth;
    }
}
