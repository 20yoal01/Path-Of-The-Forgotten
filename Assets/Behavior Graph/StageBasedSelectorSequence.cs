using System;
using Unity.Behavior;
using UnityEngine;
using Composite = Unity.Behavior.Composite;
using Unity.Properties;
using System.Collections.Generic;
using System.Linq;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "StageBasedSelector", story: "[Boss] Execute [IncludedTasks] based on [Stage] inside [Arena]", category: "Flow", id: "26b66427af72a9e846d9b577fb8df2b4")]
public partial class StageBasedSelectorSequence : Composite
{
    [SerializeReference] public BlackboardVariable<GameObject> Boss;
    [SerializeReference] public BlackboardVariable<List<string>> IncludedTasks;
    [SerializeReference] public BlackboardVariable<int> Stage;
    [SerializeReference] public BlackboardVariable<GameObject> Arena;
    int m_RandomIndex = 0;

    private BoxCollider2D _bossRoomCollider;

    enum ArenaLocation {Corner, Center, Closeup}


    ArenaLocation GetRelativeArenaLocation()
    {
        // Cache the boss transform and BoxCollider2D bounds
        var bossTransform = Boss.Value.transform;
        var arenaBounds = _bossRoomCollider;

        // Calculate relative position within the arena in world space
        var relativeArenaPosX = bossTransform.position.x - arenaBounds.size.x;

        // Get facing direction from local scale
        var facing = Mathf.Sign(bossTransform.localScale.x);

        // Normalize the position using half the arena width
        var normalizedArenaPosX = relativeArenaPosX / (arenaBounds.size.x / 2) * facing;

        // Determine arena location
        if (normalizedArenaPosX < -0.33f)
            return ArenaLocation.Corner;
        if (normalizedArenaPosX <= 0.33f) // Inclusive boundary
            return ArenaLocation.Center;
        else
            return ArenaLocation.Closeup;
    }
    


    /// <inheritdoc cref="OnStart" />
    protected override Status OnStart()
    {
        if (!Arena.Value.TryGetComponent(out _bossRoomCollider))
        {
            Debug.LogError("Agent Doesn't have a Animator!");
            return Status.Failure;
        }

        var arenaLocation = GetRelativeArenaLocation();
        var tasks = arenaLocation switch
        {
            ArenaLocation.Corner => 1,
            ArenaLocation.Center => 2,
            ArenaLocation.Closeup => 3,
        };
        Debug.Log(arenaLocation.ToString());

        List<int> childrenExecutionOrder = IncludedTasks.Value[Stage.Value].Split(",").Select(int.Parse).ToList();
        m_RandomIndex = UnityEngine.Random.Range(0, childrenExecutionOrder.Count);
        m_RandomIndex = childrenExecutionOrder[m_RandomIndex];
        if (m_RandomIndex < Children.Count)
        {
            var status = StartNode(Children[m_RandomIndex]);
            if (status == Status.Success || status == Status.Failure)
                return status;

            return Status.Waiting;
        }

        return Status.Success;
    }



    /// <inheritdoc cref="OnUpdate" />
    protected override Status OnUpdate()
    {
        var status = Children[m_RandomIndex].CurrentStatus;
        if (status == Status.Success || status == Status.Failure)
            return status;

        return Status.Waiting;
    }
}
