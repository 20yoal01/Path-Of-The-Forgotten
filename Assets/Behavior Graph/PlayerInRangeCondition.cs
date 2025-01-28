using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "PlayerInRange", story: "[Player] in [MeleeRange]", category: "Conditions", id: "495acd911840552b2fb5576f69f21be4")]
public partial class PlayerInRangeCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Player;
    [SerializeReference] public BlackboardVariable<BoxCollider2D> MeleeRange;

    public override bool IsTrue()
    {
        // Check if Player and MeleeRange are set
        if (Player.Value == null || MeleeRange.Value == null)
            return false;

        // Get the BoxCollider2D's position, size, and rotation
        Vector2 meleeRangeCenter = MeleeRange.Value.bounds.center;
        Vector2 meleeRangeSize = MeleeRange.Value.bounds.size;

        // Check for overlaps using Physics2D
        Collider2D hit = Physics2D.OverlapBox(
            meleeRangeCenter,
            meleeRangeSize,
            0f, // No rotation
            LayerMask.GetMask("Player") // Make sure to set the Player layer
        );

        // Return true if the Player is in the melee range
        return hit != null && hit.gameObject == Player.Value;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
