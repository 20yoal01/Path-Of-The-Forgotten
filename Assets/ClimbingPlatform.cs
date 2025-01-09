using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingPlatform : MonoBehaviour
{
    [System.Serializable]
    public class LeverData
    {
        public Transform point;
        public GameObject AttachedLever;
    }

    public List<LeverData> leverData;
    public float moveSpeed = 2f;
    private Vector3 currentPosition;
    private Vector3 previousPosition;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        for (int i = 0; i < leverData.Count; i++)
        {
            Vector3 nextPosition = leverData[i].point.position;
            if (i == 0)
            {
                currentPosition = nextPosition;
            }

            GameObject AttachedLever = leverData[i].AttachedLever;
            if (AttachedLever != null)
            {
                LeverTrigger leverTrigger = AttachedLever.gameObject.GetComponent<LeverTrigger>();
                leverTrigger.InitializeWallPlatform(TogglePlatform, nextPosition);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if ((transform.position - currentPosition).sqrMagnitude < 0.01f)
            return;

        Vector3 targetPosition = new Vector3(transform.position.x, currentPosition.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

    }

    // Toggles the target position of the platform
    public void TogglePlatform(Vector3 nextPosition)
    {
        previousPosition = currentPosition;
        currentPosition = nextPosition;
    }

}
