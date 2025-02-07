using System.Collections;
using System.Collections.Generic;
using UnityEngine;



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

    // Method to start the chase cooldown timer
    public void StartChaseCooldown()
    {
        chaseTimer = ChaseCooldown;
        StartCoroutine(ChaseCooldownTimer());
    }

    // Method to start the meteorite cooldown timer
    public void StartMeteoriteCooldown()
    {
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
}
