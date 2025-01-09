using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Decisions/KilledDecision")]
public class KilledDecision : Decision
{
    public override bool Decide(BaseStateMachine stateMachine)
    {
        return !stateMachine.GetComponent<Animator>().GetBool(AnimationString.isAlive);
    }
}
