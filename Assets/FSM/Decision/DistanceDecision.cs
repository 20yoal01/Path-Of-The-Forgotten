using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Decisions/DistanceDecision")]
public class DistanceDecision : Decision
{
    GameObject target;
    public string targetTag;
    public float distanceThreshhold = 3f;

    public override bool Decide(BaseStateMachine stateMachine)
    {
        if (target == null) target = GameObject.FindWithTag(targetTag);

        bool attackRange = stateMachine.GetComponent<Mushroom>().inRangedAttackRange;

        return !attackRange;
    }
}
