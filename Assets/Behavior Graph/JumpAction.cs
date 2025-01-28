using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using DG.Tweening;
using Cinemachine;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Jump", story: "[Agent] Performs a jump", category: "Action", id: "9b512b8f2a8c96663dc561d4916bde82")]
public partial class JumpAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    [SerializeReference] public BlackboardVariable<float> jumpForce;
    [SerializeReference] public BlackboardVariable<float> horizontalForce;
    [SerializeReference] public BlackboardVariable<float> jumpTime;

    [SerializeReference] public BlackboardVariable<float> buildupTime;
    [SerializeReference] public BlackboardVariable<string> animationAttackType;
    [SerializeReference] public BlackboardVariable<string> animationAttackTrigger;
    [SerializeReference] public BlackboardVariable<string> animationMain;

    [SerializeReference] public BlackboardVariable<GameObject> jumpEffect;
    [SerializeReference] public BlackboardVariable<Vector3> effectOffset;

    private Rigidbody2D _agentRb;
    private Animator _agentAnimator;
    private TouchingDirections _agentTD;

    private bool hasLanded;

    private Tween buildupTween;
    private Tween jumpTween;

    protected override Status OnStart()
    {
        if (ReferenceEquals(Agent?.Value, null) || ReferenceEquals(Target?.Value, null))
        {
            return Status.Failure;
        }

        if (!Agent.Value.TryGetComponent(out _agentRb))
        {
            Debug.LogError("Agent Doesn't have a RigidBody!");
            return Status.Failure;
        }

        if (!Agent.Value.TryGetComponent(out _agentAnimator))
        {
            Debug.LogError("Agent Doesn't have a Animator!");
            return Status.Failure;
        }


        buildupTween = DOVirtual.DelayedCall(buildupTime, StartJump, false);
        _agentAnimator.SetTrigger(animationAttackTrigger);
        _agentAnimator.SetInteger(animationAttackType, ((int)FlameLordAttackType.Dash));

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return hasLanded ? Status.Success : Status.Running;
    }

    protected override void OnEnd()
    {
        buildupTween?.Kill();
        hasLanded = false;
        jumpTween?.Kill();
    }

    private void StartJump()
    {
        if (!string.IsNullOrEmpty(animationMain))
            _agentAnimator.SetTrigger(animationMain);

        var direction = Agent.Value.transform.localScale.x > 0 ? 1 : -1;
        _agentRb.AddForce(new Vector2(horizontalForce.Value * direction, jumpForce.Value), ForceMode2D.Impulse);

        if (jumpEffect != null)
        {
            // Calculate the offset based on direction
            var offset = new Vector3(effectOffset.Value.x * direction, effectOffset.Value.y, effectOffset.Value.z);

            // Play the effect with the adjusted offset
            EffectManager.Instance.PlayEffect(jumpEffect, _agentRb.transform.position + offset, direction);
        }
        
        jumpTween = DOVirtual.DelayedCall(jumpTime, () =>
        {
            _agentRb.linearVelocity = Vector2.zero;
            hasLanded = true;
            CinemachineImpulseSource source = GameObject.FindGameObjectWithTag("Player").GetComponent<CinemachineImpulseSource>();
            source.m_DefaultVelocity = new Vector3(0.3f, 0.3f);
            CameraShakeManager.instance.CameraShake(source);
        }, false);
        
    }
}

