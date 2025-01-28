using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour
{
    public DetectionZone rangedZone;
    public DetectionZone meleeZone;
    public DetectionZone biteZone;

    public bool inRangedAttackRange;
    public bool inMeleeAttackRange;
    public bool inBiteAttackRange;

    // Update is called once per frame
    void Update()
    {
        inRangedAttackRange = rangedZone.DetectedColliders.Count > 0;
        inMeleeAttackRange = meleeZone.DetectedColliders.Count > 0;
        inBiteAttackRange = biteZone.DetectedColliders.Count > 0;
    }
}
