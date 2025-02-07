using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using DG.Tweening;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "SpawnBoss", story: "[Agent] Spawns in Front of Target", category: "Action", id: "79f733454022698337aa1077212a6644")]
public partial class SpawnBossAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<bool> SpawnBoss;
    [SerializeReference] public BlackboardVariable<float> FadeTime;
    protected override Status OnStart()
    {
        var spriteRenderer = Agent.Value.GetComponent<SpriteRenderer>();

        spriteRenderer.DOFade(1, FadeTime);

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
        SpawnBoss.Value = false;
    }
}

