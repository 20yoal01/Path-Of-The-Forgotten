using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Death", story: "[Agent] death", category: "Conditions", id: "ca0ced280668e55cb787475aeab9c00a")]
public partial class DeathCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;

    private Damageable _agentDamageable;
    public override bool IsTrue()
    {
        if (!Agent.Value.TryGetComponent(out _agentDamageable)){
            Debug.LogError("Could not find a Damageable Component tied to this Agent!");
            return true;
        }

        return !_agentDamageable.IsAlive;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
