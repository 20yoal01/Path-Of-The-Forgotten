using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Decisions/InMeleeBiteAttackDecision")]
public class InMeleeBiteAttackDecision : Decision
{
    public override bool Decide(BaseStateMachine stateMachine)
    {
        return stateMachine.GetComponent<Mushroom>().inBiteAttackRange;
    }
}
