using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Decisions/InMeeleAndBiteRange")]
public class InMeeleAndBiteRange : Decision
{
    public override bool Decide(BaseStateMachine stateMachine)
    {
        bool biteRange = stateMachine.GetComponent<Mushroom>().inBiteAttackRange;
        bool attackRange = stateMachine.GetComponent<Mushroom>().inMeleeAttackRange;

        Animator animator = stateMachine.GetComponent<Animator>();
        bool attackPerform = animator.GetBool("attackPerform");

        if (attackRange && !biteRange && !attackPerform)
            return true;

        return false;
    }
}
