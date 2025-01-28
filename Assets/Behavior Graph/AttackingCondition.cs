using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Attacking", story: "[Agent] attacking", category: "Conditions", id: "806e7b1c86d60c4a3ee9acaf74cabfe8")]
public partial class AttackingCondition : Condition
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