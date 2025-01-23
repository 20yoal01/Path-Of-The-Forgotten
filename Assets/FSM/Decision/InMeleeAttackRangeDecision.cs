using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Decisions/InMeleeAttackRangeDecision")]
public class InMeleeAttackRangeDecision : Decision
{    
    public override bool Decide(BaseStateMachine stateMachine)
    {
        Animator animator = stateMachine.GetComponent<Animator>();
        bool attackPerform = animator.GetBool("attackPerform");

        return stateMachine.GetComponent<Mushroom>().inMeleeAttackRange && !attackPerform;
    }
}
