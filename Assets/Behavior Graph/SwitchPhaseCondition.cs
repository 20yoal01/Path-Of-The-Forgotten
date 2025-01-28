using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Switch Phase", story: "[Agent] falls below {procent} [HP]", category: "Conditions", id: "819acd5e42ac61a47d2bccb62d9e707f")]
public partial class SwitchPhaseCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<int> HP;
    private Damageable _agentDamageable;
    public override bool IsTrue()
    {
        if (!Agent.Value.TryGetComponent(out _agentDamageable))
        {
            Debug.LogError("Could not find a Damageable Component tied to this Agent!");
            return true;
        }

        float HP_threshold = ((HP / 100f) * _agentDamageable.MaxHealth);
        bool underThreshHold = (_agentDamageable.Health) <= HP_threshold;

        return underThreshHold;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
