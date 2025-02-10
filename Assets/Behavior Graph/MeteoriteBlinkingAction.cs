using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using DG.Tweening;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "MeteoriteBlinking", story: "[Agent] attacks [Target] with [Meteorite] inside [BossRoom]", category: "Action", id: "016aa53e6cd01f771242a49cee5a484c")]
public partial class MeteoriteBlinkingAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Agent;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<GameObject> Meteorite;
    [SerializeReference] public BlackboardVariable<GameObject> BossRoom;

    [SerializeReference] public BlackboardVariable<float> fadeTime;

    [SerializeReference] public BlackboardVariable<float> jumpTime;
    [SerializeReference] public BlackboardVariable<float> waitIntervall;

    [SerializeReference] public BlackboardVariable<float> meteoriteTimer;
    [SerializeReference] public BlackboardVariable<int> meteoriteCooldown;

    private Rigidbody2D _agentRb;
    private Animator _agentAnimator;
    private TouchingDirections _agentTD;
    private BoxCollider2D _bossRoomCollider;
    private Damageable _agentDamageable;
    private ParticleSystem meteoriteParticleSystem;
    private MeteoriteShower meteoriteShower;
    private FlameLord _agentFlameLord;

    private bool hasInitialized = false;
    private bool hasLanded;
    float defaultGravity;
    private Vector3 positionBeforeAttack;

    private Tween jumpTween;
    private Tween buildUpTween;

    private bool tookDamage;
    private int HPWhenEnteredAction;
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
            Debug.LogError("Agent Doesn't have a Collider!");
            return Status.Failure;
        }

        if (!Agent.Value.TryGetComponent(out _agentDamageable))
        {
            Debug.LogError("Agent Doesn't have a Damageable!");
            return Status.Failure;
        }

        if (!Agent.Value.TryGetComponent(out _agentFlameLord))
        {
            Debug.LogError("Could not retreive Flame Lord script");
        }

        if (!hasInitialized)
        {
            Initialize();
        }

        _agentAnimator.SetBool(AnimationString.canMove, false);
        tookDamage = false;
        HPWhenEnteredAction = _agentDamageable.Health;

        if (meteoriteParticleSystem == null)
        {
            GameObject meteoriteInstance = UnityEngine.Object.Instantiate(Meteorite.Value, Meteorite.Value.transform.position, Quaternion.identity);

            if (!meteoriteInstance.TryGetComponent(out meteoriteParticleSystem))
            {
                Debug.LogError("No ParticleSystem found in instantiated Meteorite!");
                return Status.Failure;
            }
            if (!meteoriteInstance.TryGetComponent(out meteoriteShower))
            {
                Debug.LogError("No ParticleSystem found in instantiated Meteorite!");
                return Status.Failure;
            }
        }
        meteoriteParticleSystem.Play();
        meteoriteShower.PlayStackedSounds();

        positionBeforeAttack = Agent.Value.transform.position;

        if (jumpTween != null && jumpTween.IsActive())
        {
            jumpTween.Kill();
        }

        jumpTween = DOVirtual.DelayedCall(jumpTime, StartBuildup, false);

        return Status.Running;
    }

    private void Initialize()
    {
        defaultGravity = _agentRb.gravityScale;
    }
    void StartBuildup()
    {
        if (tookDamage) return;

        // Stop gravity and velocity for precise control during blink
        _agentRb.gravityScale = 0;
        _agentRb.linearVelocity = Vector2.zero;

        // Get boss room bounds
        Vector2 roomSize = _bossRoomCollider.size;
        Vector2 roomCenter = _bossRoomCollider.bounds.center;

        float start_x = roomCenter.x - roomSize.x;
        float end_x = roomCenter.x + roomSize.x;

        float start_y = roomCenter.y - roomSize.y;
        float end_y = roomCenter.y + roomSize.y;

        // Generate random position within the entire room
        float blinkX = UnityEngine.Random.Range(start_x, end_x);
        float blinkY = UnityEngine.Random.Range(start_y, end_y);

        var blinkPosition = new Vector2(blinkX, blinkY);

        Vector2 playerPosition = Target.Value.transform.position;
        float distanceToPlayer = Vector2.Distance(blinkPosition, playerPosition);
        if (distanceToPlayer < 10f)
        {
            Vector2 directionToPlayer = (blinkPosition - playerPosition).normalized;
            blinkPosition = playerPosition + directionToPlayer * 5f;
        }

        // Fade out the sprite renderer to make the enemy disappear
        var spriteRenderer = Agent.Value.GetComponent<SpriteRenderer>();
        
        spriteRenderer.DOFade(0, fadeTime)
            .OnComplete(() =>
            {
                // Move to the blink position
                Agent.Value.transform.position = blinkPosition;

                // Wait for a short interval before fading back in
                DOVirtual.DelayedCall(fadeTime, () =>
                {
                    spriteRenderer.DOFade(1, 0.5f)
                        .OnComplete(() =>
                        {
                            if (tookDamage) return;

                            // Start the next teleport sequence after the wait interval
                            _agentAnimator.SetTrigger("buff");
                            buildUpTween = DOVirtual.DelayedCall(waitIntervall, StartBuildup);
                        });
                });
            });
    }



    protected override Status OnUpdate()
    {
        if (_agentDamageable.Health != HPWhenEnteredAction)
        {
            tookDamage = true;
        }

        // Allow all tweens to complete before returning success
        if (tookDamage && (jumpTween == null || !jumpTween.IsActive()) && (buildUpTween == null || !buildUpTween.IsActive()))
        {
            return Status.Success;
        }


        return Status.Running;
    }

    protected override void OnEnd()
    {
        var spriteRenderer = Agent.Value.GetComponent<SpriteRenderer>();

        spriteRenderer.DOFade(0, fadeTime)
            .OnComplete(() =>
            {
                // Check if player is standing at the original position
                if (Vector2.Distance(Target.Value.transform.position, positionBeforeAttack) < 1f)
                {
                    positionBeforeAttack.x += (UnityEngine.Random.value > 0.5f ? 3f : -3f);
                }

                positionBeforeAttack.y = -126;
                // Move back to the stored pre-attack position
                Agent.Value.transform.position = positionBeforeAttack;

                // Fade back in
                spriteRenderer.DOFade(1, 0.5f);
                _agentRb.gravityScale = defaultGravity;
            });
        if (meteoriteShower != null)
        {
            meteoriteShower.StopStackedSounds();
            meteoriteShower.PlayMeteoriteEndingSound();
        }
        if (meteoriteParticleSystem != null)
        {
            meteoriteParticleSystem.Stop();
        }
        jumpTween?.Kill();
        buildUpTween?.Kill();

        _agentFlameLord?.StartMeteoriteCooldown();
    }
}

