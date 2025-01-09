using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Decisions/InRangedAttackDecision")]
public class InRangedAttackDecision : Decision
{    
    public override bool Decide(BaseStateMachine stateMachine)
    {
        bool rangedRange = stateMachine.GetComponent<Mushroom>().inRangedAttackRange;
        bool meleeRange = stateMachine.GetComponent<Mushroom>().inMeleeAttackRange;

        if (rangedRange && !meleeRange)
            return true;

        return false;
    }
}
