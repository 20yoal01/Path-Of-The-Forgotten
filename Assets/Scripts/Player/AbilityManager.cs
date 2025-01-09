using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum Ability
{
    DoubleJump, Dash, Bow, WallClimb, ArrowBarrage
}

public class AbilityManager : MonoBehaviour
{
    public static readonly Dictionary<Ability, string> AbilityKeybinds = new Dictionary<Ability, string>
    {
        { Ability.DoubleJump, "Space/Z to double jump" },
        { Ability.Dash, "Shift to dash" },
        { Ability.Bow, "V to shoot | UP + LEFT/RIGHT to aim diagonally" },
        { Ability.WallClimb, "Right/Left arrow to climb" },
        { Ability.ArrowBarrage, "G to activate. Throws a barrage of arrows (5 arrows required to activate)"},
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
