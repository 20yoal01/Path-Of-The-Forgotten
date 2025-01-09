using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FSM/Decisions/WaitTimeDecision")]
public class WaitTimeDecision : Decision
{
    public float waitTime = 3f;
    float timer = 0;

    public override bool Decide(BaseStateMachine stateMachine)
    {
        timer += Time.deltaTime;
        if (timer > waitTime)
        {
            timer = 0;
            return true;
        }
        else
        {
            return false;
        }
    }
}
