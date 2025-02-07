using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CanMove", story: "[Agent] Can Move", category: "Conditions", id: "e314b6327f217e4b393116a4de207fad")]
public partial class CanMoveCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    private Animator _agentAnimator;

    public override bool IsTrue()
    {
        if (!Agent.Value.TryGetComponent(out _agentAnimator))
        {
            Debug.Log("Could not find a Animator in your Agent object");
            return false;
        }

        return _agentAnimator.GetBool(AnimationString.canMove);
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
