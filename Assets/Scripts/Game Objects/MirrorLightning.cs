using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public enum PrisonLevel
{
    Prison1, Prison2,
}

public class MirrorLightning : MonoBehaviour
{
    public Light2D Light;
    public float targetIntensity = 1f;
    public float targetFalloff = 1f;
    public float speed = 1f;

    private float initialIntensity;
    private float initialFalloff;
    private bool isTurningOn = false;
    private float t = 0f;
    private bool activateOnShoot;
    private Animator animator;

    public MirrorLightning mirrorToActivate;

    public PrisonLevel level;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (Light != null)
        {
            initialIntensity = Light.intensity;
            initialFalloff = Light.falloffIntensity;
        }

        if ((GameManager.Instance.LeverPrison1 && level == PrisonLevel.Prison1) ||
           (GameManager.Instance.LeverPrison2 && level == PrisonLevel.Prison2))
        {
            isTurningOn = true;

            // Activate the linked mirror if applicable
            if (mirrorToActivate != null)
            {
                mirrorToActivate.isTurningOn = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool LeverPrison = false;
        if (level == PrisonLevel.Prison1)
        {
            LeverPrison = GameManager.Instance.LeverPrison1;
        }
        else if (level == PrisonLevel.Prison2)
        {
            LeverPrison = GameManager.Instance.LeverPrison2;
        }

        if (collision.CompareTag("PlayerArrow") && !LeverPrison)
        {
            if (mirrorToActivate != null)
            {
                mirrorToActivate.isTurningOn = true;
                mirrorToActivate.activateOnShoot = true;
            }
            else
            {
                isTurningOn = true;
                activateOnShoot = true;
            }

            if (level == PrisonLevel.Prison1)
            {
                GameManager.Instance.LeverPrison1 = true;
            }
            else if (level == PrisonLevel.Prison2)
            {
                GameManager.Instance.LeverPrison2 = true;
            }

            
        }
    }

    private void FixedUpdate()
    {
        string active_string = "active";

        if (activateOnShoot)
            animator.SetBool(active_string, true);

        if (isTurningOn && Light != null)
        {
            t += Time.deltaTime * speed;


            Light.intensity = Mathf.Lerp(initialIntensity, targetIntensity, t);
            Light.falloffIntensity = Mathf.Lerp(targetFalloff, initialIntensity, 1 - t);
            Light.enabled = true;
            if (t >= 1f)
            {
                isTurningOn = false;
                t = 0f; 
            }
        }
    }
}
