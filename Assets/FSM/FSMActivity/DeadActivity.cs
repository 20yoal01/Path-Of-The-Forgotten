using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Activity/DeadActivity")]
public class DeadActivity : Activity
{

    public override void Enter(BaseStateMachine stateMachine)
    {
        stateMachine.GetComponent<Rigidbody2D>().linearVelocity = Vector3.zero;
    }

    public override void Execute(BaseStateMachine stateMachine)
    {
    }

    public override void Exit(BaseStateMachine stateMachine)
    {

    }
}
