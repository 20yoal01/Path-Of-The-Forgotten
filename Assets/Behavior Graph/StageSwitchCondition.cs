using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "StageSwitch", story: "Time to [Switch]", category: "Conditions", id: "95f93742594bcb15aaed7fc62ae31ee4")]
public partial class StageSwitchCondition : Condition
{
    [SerializeReference] public BlackboardVariable<bool> Switch;

    public override bool IsTrue()
    {

        return true;
    }

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
