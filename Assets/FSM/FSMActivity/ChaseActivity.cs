using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Activity/ChaseActivity")]
public class ChaseActivity : Activity
{
    GameObject target;
    public string targetTag;
    public float speed = 1f;

    public override void Enter(BaseStateMachine stateMachine)
    {
        target = GameObject.FindWithTag(targetTag);
        stateMachine.GetComponent<Animator>().SetBool("roaming", true);
    }

    public override void Execute(BaseStateMachine stateMachine)
    {
        var RigidBody = stateMachine.GetComponent<Rigidbody2D>();
        var SpriteRenderer = stateMachine.GetComponent<SpriteRenderer>();
        var Transform = stateMachine.GetComponent<Transform>();

        Vector2 dir = (target.transform.position + target.transform.forward - Transform.position).normalized;
        RigidBody.velocity = new Vector2(dir.x * speed, RigidBody.velocity.y);

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
    }

    public override void Exit(BaseStateMachine stateMachine)
    {

    }
}
