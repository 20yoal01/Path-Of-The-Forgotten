using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "SwitchStageCondition", story: "[Agent] falls below [HP] will modify [stageswitch] based on [wishstage]", category: "Conditions", id: "0ced9cdfa722b6824555113d242243ef")]
public partial class SwitchStageCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<int> HP;
    [SerializeReference] public BlackboardVariable<bool> Stageswitch;
    [SerializeReference] public BlackboardVariable<int> Wishstage;

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

        if (underThreshHold && Wishstage < 1)
        {
            Stageswitch.Value = true;
        }
        else
        {
            underThreshHold = false;
        }
            


        return underThreshHold;
    }
    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
