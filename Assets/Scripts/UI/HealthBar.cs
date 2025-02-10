using DG.Tweening;
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
    public RectTransform healthFill;
    public RectTransform healthBackground;
    public Animator healthAnimator;

    [Header("Player Settings")]
    public string TagToAttach;

    [Header("Animation Settings")]
    public float animationDuration = 1f;
    public float sizeIncreasePerUpgrade = 45f;
    public float positionOffsetPerUpgrade = 80f;

    Damageable playerDamageable;
    PlayerController playerController;

    bool upgradeOnce = true;
    private int startMaxHP;

    Vector2 initialHealthFillPosition;
    Vector2 initialHealthFillSize;
    Vector2 initialHealthBackgroundPosition;
    Vector2 initialHealthBackgroundSize;

    private void Awake()
    {
        if (TagToAttach == "Player")
            CacheInitialValues();
        AttachToPlayer();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (playerDamageable == null) return;
        UpdateHealthUI(playerDamageable.Health, playerDamageable.MaxHealth);
    }

    private void OnEnable()
    {
        if (playerDamageable != null)
        {
            if (TagToAttach == "Player")
                playerController.healthUpgradedEvent.AddListener(HealthUpgradeState);
            playerDamageable.healthChanged.AddListener(OnPlayerHealthChanged);
        }
            
    }

    private void OnDisable()
    {
        if (playerDamageable != null)
        {
            if (TagToAttach == "Player")
                playerController.healthUpgradedEvent.RemoveListener(HealthUpgradeState);
            playerDamageable.healthChanged.RemoveListener(OnPlayerHealthChanged);
            
            ResetHealthUpgrade();
        }  
    }

    private void CacheInitialValues()
    {
        initialHealthFillPosition = healthFill.position;
        initialHealthFillSize = healthFill.sizeDelta;
        initialHealthBackgroundPosition = healthBackground.position;
        initialHealthBackgroundSize = healthBackground.sizeDelta;
    }

    private void AttachToPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag(TagToAttach);
        if (player == null)
        {
            Debug.LogWarning("No player found in the scene. Make sure it has the correct tag.");
            return;
        }

        playerDamageable = player.GetComponent<Damageable>();
        playerController = player.GetComponent<PlayerController>();

        if (playerDamageable != null && TagToAttach == "Player")
        {
            startMaxHP = playerDamageable.MaxHealth;
        }
        else
        {
            startMaxHP = -1000;
        }
    }

    private void UpdateHealthUI(float newHealth, float maxHealth)
    {
        float clampedHealth = Mathf.Clamp(newHealth, 0, maxHealth);

        healthSlider.DOValue(clampedHealth / maxHealth, 0.5f).SetEase(Ease.OutCubic);
        if (healthBarText != null)
        {
            healthBarText.text = $"HP {newHealth} / {maxHealth}";
        }
    }

    private void OnPlayerHealthChanged(float newHealth, float maxHealth)
    {
        if (playerDamageable == null) return;

        UpdateHealthUI(newHealth, maxHealth);
    }

    public void HealthUpgradeState(bool shouldUpgrade)
    {
        if (shouldUpgrade)
        {
            TriggerHealthUpgrade();
        }
        else
        {
            ResetHealthUpgrade();
        }
    }

    private void TriggerHealthUpgrade()
    {
        if (healthAnimator != null)
        {
            healthAnimator.SetBool("healthUpgrade", upgradeOnce);
            healthAnimator.SetBool("healthUpgradeNoAnim", !upgradeOnce);
        }

        float upgradeMultiplier = (playerDamageable.MaxHealth - startMaxHP) / (float)startMaxHP;

        AnimateHealthBar(
            sizeIncreasePerUpgrade
        );
    }

    public void ResetHealthUpgrade()
    {
        if (healthAnimator != null)
        {
            healthAnimator.SetBool("healthUpgrade", false);
        }

        AnimateHealthBar(0, true);
    }

    private void AnimateHealthBar(float sizeIncrease, bool reset = false)
    {
        AnimateElement(healthFill, initialHealthFillSize, sizeIncrease, reset);
        AnimateElement(healthBackground, initialHealthBackgroundSize, sizeIncrease, reset);
    }

    private void AnimateElement(RectTransform element, Vector2 initialSize, float sizeIncrease, bool reset)
    {
        Vector2 targetSize = reset
            ? initialSize
            : new Vector2(initialSize.x + sizeIncrease, initialSize.y);

        element.DOSizeDelta(targetSize, animationDuration).SetEase(Ease.InCubic);
    }
}
