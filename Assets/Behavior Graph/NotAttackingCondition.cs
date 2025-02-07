using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "NotAttacking", story: "[Agent] is not attacking", category: "Conditions", id: "9d50b91a110159d87aff3b0b07629041")]
public partial class NotAttackingCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    private Animator sad;
    public override bool IsTrue()
    {
        if (!Agent.Value.TryGetComponent(out sad))
        {
            Debug.LogError("Cannot find Animator in Agent");
            return false;
        }

        bool isAttacking = sad.GetBool(AnimationString.attackTrigger);

        if (isAttacking)
        {
            Debug.Log("Attacking");
        }

        return !isAttacking;
    }
    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
