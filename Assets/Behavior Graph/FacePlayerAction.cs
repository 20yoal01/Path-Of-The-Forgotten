using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "FacePlayer", story: "[Agent] Faces [Player]", category: "Action", id: "2148d983eab1f7306424b0edc95b844d")]
public partial class FacePlayerAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Player;

    private float baseScaleX;
    private bool hasInitialized = false; // Initialization flag
    private Animator _animator;

    private Status Initialize()
    {
        hasInitialized = true;
        baseScaleX = Agent.Value.transform.localScale.x;

        if (!Agent.Value.TryGetComponent(out _animator))
        {
            Debug.LogError("Agent Doesn't have a Animator!");
            return Status.Failure;
        }

        return Status.Running;
    }

    protected override Status OnStart()
    {
        if (Agent?.Value == null || Player?.Value == null)
        {
            Debug.LogError("Agent or Player is not assigned in Blackboard.");
            return Status.Failure;
        }

        if (!hasInitialized)
        {
            return Initialize();
        }
        
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        var agentPosition = Agent.Value.transform.position.x;
        var playerPosition = Player.Value.transform.position.x;
        var scale = Agent.Value.transform.localScale;

        // Only update if the direction needs to change
        if ((agentPosition > playerPosition && scale.x > 0) || (agentPosition < playerPosition && scale.x < 0))
        {
            scale.x = agentPosition > playerPosition ? -Mathf.Abs(baseScaleX) : Mathf.Abs(baseScaleX);
            Agent.Value.transform.localScale = scale;
        }

        return Status.Success;
    }
}

