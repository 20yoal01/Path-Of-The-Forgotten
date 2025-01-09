using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LeverTrigger : MonoBehaviour
{
    private UnityAction onLeverActivated;
    private UnityAction<Vector3> onWallLeverActivated;
    private Animator animator;
    public int leverCloseCoolDown;

    private bool leverOff;
    private bool platformToggle;
    private bool wallPlatformToggle;
    private Vector3 nextPosition;

    public void Initialize(UnityAction action, int leverCloseCoolDown)
    {
        this.leverCloseCoolDown = leverCloseCoolDown;
        onLeverActivated += action;
    }

    public void InitializePlatform(UnityAction action)
    {
        onLeverActivated += action;
        platformToggle = true;
    }

    public void InitializeWallPlatform(UnityAction<Vector3> action, Vector3 nextPosition)
    {
        onWallLeverActivated += action;
        wallPlatformToggle = true;
        this.nextPosition = nextPosition;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        leverOff = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerArrow"))
        {
            onLeverActivated?.Invoke();

            if (!platformToggle && !wallPlatformToggle)
            {
                OpenLever();
            }
            else
            {
                if (wallPlatformToggle)
                    onWallLeverActivated.Invoke(nextPosition);
                leverOff = !leverOff;
                ToggleLever(leverOff);
            }
        }
    }

    public void ToggleLever(bool open)
    {
       animator.SetBool("leverOpen", !leverOff);
    }

    public void OpenLever()
    {
        if (!leverOff)
            return;

        animator.SetBool("leverOpen", true);
        
        if (leverCloseCoolDown == 0)
        {
            return;
        }
        StartCoroutine(CloseLever());
    }

    public IEnumerator CloseLever()
    {
        leverOff = false;
        yield return new WaitForSeconds(leverCloseCoolDown);
        animator.SetBool("leverOpen", false);
        leverOff = true;
    }
}
