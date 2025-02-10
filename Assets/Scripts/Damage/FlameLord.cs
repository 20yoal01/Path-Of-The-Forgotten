using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public enum FlameLordAttackType
{
    Melee = 0,
    Chain = 1,
    Ranged = 2,
    Dash = 3
}

public class FlameLord : MonoBehaviour
{
    public int ChaseCooldown; // Time before the flame lord can chase again
    public int MeteoriteCooldown; // Time before meteorite can be launched again

    public float chaseTimer;
    public float meteoriteTimer;

    BehaviorGraphAgent BGA;

    public Light2D light2D;

    public BossSetup bossSetup;

    private void Awake()
    {
        BGA = GetComponent<BehaviorGraphAgent>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (BGA != null && player != null)
        {
            BGA.SetVariableValue<GameObject>("Target", player);
        }
        else
        {
            Debug.LogError("Could not find Player");
        }
    }

    // Method to start the chase cooldown timer
    public void StartChaseCooldown()
    {
        chaseTimer = ChaseCooldown;
        StartCoroutine(ChaseCooldownTimer());
    }

    // Method to start the meteorite cooldown timer
    public void StartMeteoriteCooldown()
    {
        if (!gameObject.activeInHierarchy)
        {
            Debug.LogWarning("Cannot start meteorite cooldown because the object is inactive.");
            return;
        }

        meteoriteTimer = MeteoriteCooldown;
        StartCoroutine(MeteoriteCooldownTimer());
    }

    private IEnumerator ChaseCooldownTimer()
    {
        while (true)
        {
            if (chaseTimer > 0)
            {
                chaseTimer -= Time.deltaTime; // Decrease the timer
            }
            else
            {
                // Trigger action when cooldown is done
                Debug.Log("Chase cooldown complete!");
                yield break; // Exit the coroutine when cooldown is complete
            }
            yield return null;
        }
    }

    private IEnumerator MeteoriteCooldownTimer()
    {
        while (true)
        {
            if (meteoriteTimer > 0)
            {
                meteoriteTimer -= Time.deltaTime; // Decrease the timer
            }
            else
            {
                // Trigger action when cooldown is done
                Debug.Log("Meteorite cooldown complete!");
                yield break; // Exit the coroutine when cooldown is complete
            }
            yield return null;
        }
    }

    public void Death()
    {
        Color colorToChange;
        if (light2D != null && ColorUtility.TryParseHtmlString("#FFFFFF", out colorToChange))  // Hex color for #FF4B4B (a redish color)
        {
            ChangeLightColor(colorToChange, 1f);
        }
        else
        {
            Debug.LogError("Invalid hex color");
        }
    }
    void ChangeLightColor(Color targetColor, float duration)
    {
        Color startColor = light2D.color;
        DOVirtual.Color(startColor, targetColor, 10, (value) =>
        {
            light2D.color = value;
        }).OnComplete(() =>
        {
            if (bossSetup != null)
                bossSetup.PlayBossEndSequence();
        });
    }

}
