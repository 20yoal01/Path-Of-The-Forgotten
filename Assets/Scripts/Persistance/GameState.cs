using UnityEngine;

[CreateAssetMenu(fileName = "GameState", menuName = "Game/PersistantState", order =1)]
public class GameState : ScriptableObject
{
    public Vector3 playerPosition = Vector3.zero;
    public int playerMaxHealth = 100;
    public int playerHealth = 100;

    public bool arrowBarrage = false;
    public bool doubleJump = false;
    public bool wallJump = false;
    public bool healthUpgrade = false;
    public bool dash = false;
    public bool shootBow = false;
    public bool increasedHP = false;

    public int maxArrows = 8;

    public int scorpionsKilled;
    public int scorpionsRequired;

    public bool LeverPrison1 = false;
    public bool LeverPrison2 = false;

    public bool keyPickedUp = false;
}
