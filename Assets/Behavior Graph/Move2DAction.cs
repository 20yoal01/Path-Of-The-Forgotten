using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move2D", story: "[Agent] Navigates To [Target]", category: "Action", id: "e131dca5cae02a03fd74a7cc534424b0")]
public partial class Move2DAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    [SerializeReference] public BlackboardVariable<float> Speed = new(1.0f);
    [SerializeReference] public BlackboardVariable<float> DistanceThreshold = new(0.2f);
    [SerializeReference] public BlackboardVariable<float> SlowDownDistance = new(1.0f);

    private float _previousStoppingDistance;
    private Vector2 _lastTargetPosition;
    private Vector2 _colliderAdjustedTargetPosition;
    private float _colliderOffset;
    private Rigidbody2D _agentRb;
    private Animator _animator;

    protected override Status OnStart()
    {
        if (ReferenceEquals(Agent?.Value, null) ||ReferenceEquals(Target?.Value, null))
        {
            return Status.Failure;
        }

        return Initialize();
    }

    protected override void OnEnd()
    {
    }

    protected override Status OnUpdate()
    {
        if (ReferenceEquals(Agent?.Value, null) ||ReferenceEquals(Target, null))
        {
            return Status.Failure;
        }

        // Check if the target position has changed
        bool boolUpdateTargetPosition = !Mathf.Approximately(_lastTargetPosition.x, Target.Value.transform.position.x) || !Mathf.Approximately(_lastTargetPosition.y, Target.Value.transform.position.y);

        if (boolUpdateTargetPosition)
        {
            _lastTargetPosition = Target.Value.transform.position;
            _colliderAdjustedTargetPosition = GetPositionColliderAdjusted();
        }

        float distance = GetDistanceXY();

        if (distance <= DistanceThreshold + _colliderOffset)
        {
            return Status.Success;
        }

        float speed = Speed;
        
        if (SlowDownDistance > 0.0f && distance < SlowDownDistance)
        {
            float ratio = distance / SlowDownDistance;
            speed = Mathf.Max(0.1f, Speed * ratio);
        }

        Vector2 agentPosition = Agent.Value.transform.position;
        Vector2 toDesination = _colliderAdjustedTargetPosition - agentPosition;
        toDesination.Normalize();
        agentPosition = toDesination * speed;

        float xValocity = Mathf.Clamp(agentPosition.x + (50 * Time.deltaTime), -Speed, Speed);

        if (!_animator.GetBool(AnimationString.attackTrigger))
        {
            _agentRb.linearVelocity = new Vector2(xValocity, _agentRb.linearVelocity.y);
            _animator.SetBool(AnimationString.canMove, true);
        }
        else if (_animator.GetBool(AnimationString.attackTrigger) || !_animator.GetBool(AnimationString.canMove))
        {
            _agentRb.linearVelocity = Vector2.zero;
            _animator.SetBool(AnimationString.canMove, false);
        }
            


        return Status.Success;
    }

    protected override void OnDeserialize() => Initialize();

    private Status Initialize()
    {
        _lastTargetPosition = Target.Value.transform.position;
        _colliderAdjustedTargetPosition = GetPositionColliderAdjusted();

        if (!Agent.Value.TryGetComponent(out _agentRb))
        {
            Debug.LogError("Agent Doesn't have a RigidBody!");
            return Status.Failure;
        }

        if (!Agent.Value.TryGetComponent(out _animator))
        {
            Debug.LogError("Agent Doesn't have a Animator!");
            return Status.Failure;
        }


        // Add the extends of the colliders to the stopping distance
        _colliderOffset = 0.0f;
        Collider2D agentCollider = Agent.Value.GetComponent<Collider2D>();
        if (agentCollider != null)
        {
            Vector2 colliderExtents = agentCollider.bounds.extents;
            _colliderOffset += Mathf.Max(colliderExtents.x, colliderExtents.y);
        }

        return GetDistanceXY() <= DistanceThreshold + _colliderOffset ? Status.Success : Status.Running;
    }

    private Vector2 GetPositionColliderAdjusted()
    {
        Collider2D targetCollider = Target.Value.GetComponent<Collider2D>();

        if (targetCollider != null)
        {
            return targetCollider.ClosestPoint(Agent.Value.transform.position);
        }

        return Target.Value.transform.position;
    }

    private float GetDistanceXY() => Vector2.Distance(Agent.Value.transform.position, _colliderAdjustedTargetPosition);
}

