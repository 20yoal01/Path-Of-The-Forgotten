using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum Ability
{
    DoubleJump, Dash, Bow, WallClimb, ArrowBarrage, HealthIncrease, ArrowHeal
}

public class AbilityManager : MonoBehaviour
{
    public static readonly Dictionary<Ability, string> AbilityKeybinds = new Dictionary<Ability, string>
    {
        { Ability.DoubleJump, "Space/Z To Double Jump" },
        { Ability.Dash, "Shift To Dash" },
        { Ability.Bow, "\nV To Shoot \nUP + ←/→ To Aim Diagonally" },
        { Ability.WallClimb, " ←/→ | W/S To Climb" },
        { Ability.ArrowBarrage, "G To Activate. (Requires 5 Arrows)"},
        { Ability.HealthIncrease, "HP Increased By 25!"},
        { Ability.ArrowHeal, "Arrows Can Now Heal"}
    };

    public static string GetKeybind(Ability ability)
    {
        return AbilityKeybinds.TryGetValue(ability, out string key) ? key : "None";
    }

    public static AbilityManager Instance { get; private set; }

    private void Awake()
    {
        // Implement the singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
    }


    public UnityEvent<Ability> onAbilityUnlocked;

    public void UnlockSkill(Ability abilityType)
    {
        onAbilityUnlocked?.Invoke(abilityType);
    }
}
