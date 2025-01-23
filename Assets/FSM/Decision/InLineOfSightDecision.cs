using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Decisions/InLineOfSightDecision")]
public class InLineOfSightDecision : Decision
{
    public LayerMask layerMask;
    public float distanceThreshhold = 3f;
    Vector3 prevPosition = Vector3.zero;
    Vector3 prevDir = Vector3.zero;
    
    public override bool Decide(BaseStateMachine stateMachine)
    {
        Vector3 dir =  (stateMachine.transform.position - prevPosition).normalized;
        dir = (dir.Equals(Vector3.zero) ? prevDir : dir);
        bool attackRange = stateMachine.GetComponent<Mushroom>().inRangedAttackRange;
        prevPosition = stateMachine.transform.position;
        prevDir = dir;
        return attackRange;
    }
}
