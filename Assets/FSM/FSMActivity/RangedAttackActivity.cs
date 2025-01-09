using Assets.FSM.FSMActivity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Activity/RangedAttackActivity")]
public class RangedAttackActivity : Activity
{
    GameObject target;
    public string targetTag;

    public override void Enter(BaseStateMachine stateMachine)
    {
        target = GameObject.FindWithTag(targetTag);
        stateMachine.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        stateMachine.GetComponent<Animator>().SetBool("roaming", false);
    }

    public override void Execute(BaseStateMachine stateMachine)
    {
        var RigidBody = stateMachine.GetComponent<Rigidbody2D>();
        var SpriteRenderer = stateMachine.GetComponent<SpriteRenderer>();
        var Transform = stateMachine.GetComponent<Transform>();

        Vector2 dir = (target.transform.position + target.transform.forward - Transform.position).normalized;

        Vector3 locScale = Transform.localScale;
        if (Transform.localScale.x > 0)
        {
            // Facing the Right
            if (dir.x < 0)
            {
                Transform.localScale = new Vector3(-1 * locScale.x, locScale.y, locScale.z);
            }
        }
        else
        {
            // Facing the Left
            if (dir.x > 0)
            {
                Transform.localScale = new Vector3(-1 * locScale.x, locScale.y, locScale.z);
            }
        }

        if (stateMachine.GetComponent<RangedAttackCooldown>().CanFire)
        {
            stateMachine.GetComponent<Animator>().SetTrigger(AnimationString.fire);
            stateMachine.GetComponent<RangedAttackCooldown>().fireCounter = 0;
        }

    }

    public override void Exit(BaseStateMachine stateMachine)
    {
    }
}
