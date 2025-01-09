using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Decisions/ReachedWaypointDecision")]
public class ReachedWaypointDecision : Decision
{
    public override bool Decide(BaseStateMachine stateMachine)
    {
        return (stateMachine.GetComponent<PatrolPoints>().HasReachedPoint()) ? true : false;
    }
}
