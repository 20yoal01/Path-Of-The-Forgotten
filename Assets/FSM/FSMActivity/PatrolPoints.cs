using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPoints : MonoBehaviour
{
    public List<Transform> points;
    private Transform targetPoint;
    private int index;

    void Start()
    {
        index = 0;
        targetPoint = points[index];
    }

    public bool HasReachedPoint()
    {
        float distanceLeft = Vector3.Distance(transform.position, targetPoint.position);
        return (Vector3.Distance(transform.position, targetPoint.position) <= 2f) ? true : false;
    }

    public void SetNextTargetPoint()
    {
        index = (index == points.Count - 1) ? 0 : index + 1;
        targetPoint = points[index];
    }

    public Vector3 GetTargetPointDirection()
    {
        return (targetPoint.position - transform.position).normalized;
    }
}
