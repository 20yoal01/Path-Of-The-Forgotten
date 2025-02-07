using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using DG.Tweening;
using Cinemachine;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AirDash", story: "[Agent] Performs a AirDash", category: "Action", id: "bd6aedf6313d1ae8f927402a1581e5ea")]
public partial class AirDashAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<GameObject> BossRoom;

    [SerializeReference] public BlackboardVariable<float> fadeTime;

    [SerializeReference] public BlackboardVariable<float> jumpForce;
    [SerializeReference] public BlackboardVariable<float> dashForce;

    [SerializeReference] public BlackboardVariable<float> jumpTime;
    [SerializeReference] public BlackboardVariable<float> buildupTime;
    [SerializeReference] public BlackboardVariable<float> dashTime;

    [SerializeReference] public BlackboardVariable<string> animationAttackType;
    [SerializeReference] public BlackboardVariable<string> animationAttackTrigger;
    [SerializeReference] public BlackboardVariable<string> animationMain;

    [SerializeReference] public BlackboardVariable<GameObject> jumpEffect;
    [SerializeReference] public BlackboardVariable<Vector3> effectOffset;

    private Rigidbody2D _agentRb;
    private Animator _agentAnimator;
    private TouchingDirections _agentTD;
    private BoxCollider2D _bossRoomCollider;

    private bool hasInitialized = false;
    private bool hasLanded;
    float defaultGravity;

    private Tween buildupTween;
    private Tween jumpTween;
    private Tween dashTween;

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

        if (!BossRoom.Value.TryGetComponent(out _bossRoomCollider))
        {
            Debug.LogError("Agent Doesn't have a Animator!");
            return Status.Failure;
        }

        if (!hasInitialized)
        {
            Initialize();
        }

        // Blink to random location in area
        //_agentRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        jumpTween = DOVirtual.DelayedCall(jumpTime, StartBuildup, false);
        _agentAnimator.SetTrigger(animationAttackTrigger);
        _agentAnimator.SetInteger(animationAttackType, ((int)FlameLordAttackType.Dash));

        return Status.Running;
    }

    void StartBuildup()
    {
        // Stop gravity and velocity for precise control during blink
        _agentRb.gravityScale = 0;
        _agentRb.linearVelocity = Vector2.zero;

        // Blink Effect - Move upwards, disappear, reappear
        var originalPosition = Agent.Value.transform.localPosition;


        var roomSize = _bossRoomCollider.size;
        var roomCenter = _bossRoomCollider.bounds.center;
        float randomY = UnityEngine.Random.Range(5f, 8f);
        float maxY = roomCenter.y + roomSize.y / 2;
        float blinkY = Mathf.Min(originalPosition.y + randomY, maxY);
        float randomXOffset = UnityEngine.Random.Range(-2f, 2f); // Random X offset between -2 and 2 units
        
        float minX = roomCenter.x - roomSize.x / 2;
        float maxX = roomCenter.x + roomSize.x / 2;

        // Ensure the boss's collider doesn't exceed room bounds
        float colliderHalfWidth = _agentRb.GetComponent<PolygonCollider2D>().bounds.size.x *2;

        // Calculate the blink position on the X axis
        float blinkX = Mathf.Clamp(originalPosition.x + randomXOffset, minX + colliderHalfWidth, maxX - colliderHalfWidth);
        var blinkPosition = new Vector2(blinkX, blinkY);

        // Fade out the sprite renderer to make the enemy disappear
        var spriteRenderer = Agent.Value.GetComponent<SpriteRenderer>();
        spriteRenderer.DOFade(0, fadeTime) // Fade out over 0.5 seconds
            .OnComplete(() =>
            {
                // Move to the blink position
                Agent.Value.transform.localPosition = blinkPosition;
                Vector2 targetPos = _agentRb.position;
                var Direction = _agentRb.position.x > targetPos.x ? -1 : 1;

                // Wait for 1 second, then reappear
                DOVirtual.DelayedCall(fadeTime, () =>
                {
                    spriteRenderer.DOFade(1, 0.5f); // Fade back in over 0.5 seconds
                    buildupTween = DOVirtual.DelayedCall(buildupTime, StartDash, false);
                });
            });
    }

    protected override Status OnUpdate()
    {
        return hasLanded ? Status.Success : Status.Running;
    }

    protected override void OnEnd()
    {
        if (!hasLanded)
            _agentAnimator.SetTrigger("dashFail");
        buildupTween?.Kill();
        hasLanded = false;
        jumpTween?.Kill();
        dashTween?.Kill();
        _agentRb.gravityScale = defaultGravity;
    }

    private void StartDash()
    {
        if (!string.IsNullOrEmpty(animationMain))
            _agentAnimator.SetTrigger(animationMain);

        _agentRb.gravityScale = defaultGravity;
        var direction = Agent.Value.transform.localScale.x > 0 ? 1 : -1;
        var distance = Mathf.Abs(_agentRb.transform.position.x - Target.Value.GetComponent<Transform>().position.x);
        var downwardsDirection = distance < 4f ? -1f : -0.5f;

        _agentRb.AddForce(new Vector2(direction, downwardsDirection) * dashForce, ForceMode2D.Impulse);

        if (jumpEffect != null)
        {
            // Correctly handle the X offset based on direction (left or right)
            float offsetX = effectOffset.Value.x * direction;

            // The Y value should stay constant regardless of direction
            float offsetY = direction > 0 ? effectOffset.Value.y + 2 : effectOffset.Value.y;

            // Apply the adjusted offset
            var offset = new Vector3(offsetX, offsetY, effectOffset.Value.z);

            // Calculate the rotation based on the downwards direction
            var angle = Mathf.Atan2(downwardsDirection, direction) * Mathf.Rad2Deg;
            var effectRotation = Quaternion.Euler(0, 0, angle);

            // Play the effect with the adjusted offset and rotation
            EffectManager.Instance.PlayEffect(jumpEffect, _agentRb.transform.position + offset, direction, effectRotation);
        }

        dashTween = DOVirtual.DelayedCall(dashTime, () =>
        {
            _agentRb.linearVelocity = Vector2.zero;
            hasLanded = true;
            CinemachineImpulseSource source = GameObject.FindGameObjectWithTag("Player").GetComponent<CinemachineImpulseSource>();
            source.m_DefaultVelocity = new Vector3(0.3f, 0.3f);
            CameraShakeManager.instance.CameraShake(source);
        }, false);
    }

    private void Initialize()
    {
        defaultGravity = _agentRb.gravityScale;
    }
}

