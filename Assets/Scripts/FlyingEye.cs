using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEye : MonoBehaviour
{
    public DetectionZone biteDetectionZone;

    Animator animator;
    Rigidbody2D rb;

    public bool _hasTarget = false;
    public bool HasTarget
    {
        get { return _hasTarget; }
        private set
        {
            _hasTarget = value;
            animator.SetBool(AnimationString.hasTarget, value);
        }
    }

    // Start is called before the first frame update
    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        HasTarget = biteDetectionZone.DetectedColliders.Count > 0;
    }
}
