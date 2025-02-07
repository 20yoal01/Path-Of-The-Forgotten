using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CooldownCondition", story: "[Agent] [cooldown] exceeded", category: "Conditions", id: "6898a7c1d31b59a0e3dc28a53f7000b3")]
public partial class CooldownCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    private FlameLord _agentFlameLord;

    public override bool IsTrue()
    {
        if (_agentFlameLord.chaseTimer <= 0)
            return false;
        return true;
    }

    public override void OnStart()
    {
        if (!Agent.Value.TryGetComponent(out _agentFlameLord))
        {
            Debug.LogError("Could not retreive Flame Lord script");
        }

        if (_agentFlameLord.chaseTimer <= 0)
            _agentFlameLord.StartChaseCooldown();
    }

    public override void OnEnd()
    {
    }
}
