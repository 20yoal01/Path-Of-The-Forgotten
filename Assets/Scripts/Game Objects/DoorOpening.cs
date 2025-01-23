using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DoorName {None = 0, One, Two, Three, Four, Five, Six}

public class DoorOpening : MonoBehaviour
{
    public GameObject AttachedLever;
    private LeverTrigger leverTrigger;
    private Animator animator;

    public int doorCloseCoolDown = 5;
    private CinemachineImpulseSource impulseSource;
    public bool doorOpen;
    public enum DoorState {Closed, Open}
    public DoorState defaultState = DoorState.Closed;

    public DoorName assignedDoorName;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        if (defaultState == DoorState.Closed)
        {
            doorOpen = false;
        }
        else 
        {
            doorOpen = true;
        }

        if (assignedDoorName != DoorName.None)
        {
            GameManager.Instance.doorNameOpening.TryGetValue(assignedDoorName, out doorOpen);
        }
        
        animator.SetBool("doorOpen", doorOpen);
        animator = GetComponent<Animator>();
        if (AttachedLever != null)
        {
            leverTrigger = AttachedLever.gameObject.GetComponent<LeverTrigger>();
            if (!doorOpen && doorCloseCoolDown == 0)
            {
                leverTrigger.Initialize(OpenDoor, doorCloseCoolDown);
            }
            else
            {
                leverTrigger.Initialize(ToggleDoor, doorCloseCoolDown);
            }
        }

        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void ToggleDoor()
    {
        if (doorCloseCoolDown == 0)
        {
            return;
        }
        doorOpen = !doorOpen;
        animator.SetBool("exitDefaultState", true);
        animator.SetBool("doorOpen", doorOpen);
        StartCoroutine(ToggleDoorOpposite());
        CameraShakeManager.instance.CameraShake(impulseSource);
    }



    public IEnumerator ToggleDoorOpposite()
    {
        doorOpen = !doorOpen;
        yield return new WaitForSeconds(doorCloseCoolDown);
        animator.SetBool("doorOpen", doorOpen);
        CameraShakeManager.instance.CameraShake(impulseSource);
    }

    public void OpenDoor()
    {
        doorOpen = true;
        if (assignedDoorName != DoorName.None && doorCloseCoolDown == 0)
        {
            SaveDoor(doorOpen);
        }
        animator.SetBool("exitDefaultState", true);
        animator.SetBool("doorOpen", doorOpen);
        CameraShakeManager.instance.CameraShake(impulseSource);
    }

    public void CloseDoor()
    {
        doorOpen = false;
        animator.SetBool("exitDefaultState", true);
        animator.SetBool("doorOpen", doorOpen);
        CameraShakeManager.instance.CameraShake(impulseSource);
    }

    public void SaveDoor(bool shouldDoorOpen)
    {
        GameManager.Instance.doorNameOpening[assignedDoorName] = shouldDoorOpen;
    }
}