using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Activity/PatrolActivity")]
public class PatrolActivity : Activity
{
    public float speed = 1;

    public override void Enter(BaseStateMachine stateMachine)
    {
        var PatrolPoints = stateMachine.GetComponent<PatrolPoints>();
        var SpriteRenderer = stateMachine.GetComponent<SpriteRenderer>();
        var Animator = stateMachine.GetComponent<Animator>();
        var Transform = stateMachine.GetComponent<Transform>();
        var RigidBody = stateMachine.GetComponent<Rigidbody2D>();

        Vector3 locScale = Transform.localScale;
        if (Transform.localScale.x > 0)
        {
            // Facing the Right
            if (PatrolPoints.GetTargetPointDirection().x < 0)
            {
                Transform.localScale = new Vector3(-1 * locScale.x, locScale.y, locScale.z);
            }
        }
        else
        {
            // Facing the Left
            if (PatrolPoints.GetTargetPointDirection().x > 0)
            {
                Transform.localScale = new Vector3(-1 * locScale.x, locScale.y, locScale.z);
            }
        }

        Animator.SetBool("roaming", true);
    }

    public override void Execute(BaseStateMachine stateMachine)
    {
        var PatrolPoints = stateMachine.GetComponent<PatrolPoints>();
        var RigidBody = stateMachine.GetComponent<Rigidbody2D>();
        float x = PatrolPoints.GetTargetPointDirection().x;

        Vector2 position = RigidBody.position + new Vector2(x * speed * Time.fixedDeltaTime, 0);
        RigidBody.MovePosition(position);
    }

    public override void Exit(BaseStateMachine stateMachine)
    {
        var PatrolPoints = stateMachine.GetComponent<PatrolPoints>();
        PatrolPoints.SetNextTargetPoint();
    }
}
