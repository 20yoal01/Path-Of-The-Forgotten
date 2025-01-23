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

        Animator animator = stateMachine.GetComponent<Animator>();
        bool attackPerform = animator.GetBool("attackPerform");

        if (rangedRange && !meleeRange && !attackPerform)
            return true;

        return false;
    }
}
