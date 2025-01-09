using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSaw : MonoBehaviour
{
    public enum BarType {Vertical, Horizontal, Diagonal}
    public enum StartPoint { PointA, PointB}

    public GameObject sawBar;
    public GameObject saw;
    public float moveSpeed;

    private Vector3 pointA;
    private Vector3 pointB;

    private Vector3 currentPoint;

    public BarType barType;
    public StartPoint startPoint;
    private void Start()
    {
        Collider2D collider = sawBar.GetComponent<Collider2D>();

        if (collider == null)
        {
            Debug.LogError("No Renderer or Collider found! Please add one to calculate length.");
            return;
        }

        float lengthX = collider.bounds.size.x;
        float lengthY = collider.bounds.size.y;
        Vector3 offset = Vector3.zero;

        switch (barType)
        {
            case BarType.Horizontal:
                offset = sawBar.transform.right * (lengthX / 2); // Horizontal offset
                break;
            case BarType.Vertical:
                offset = sawBar.transform.up * (lengthY / 2); // Vertical offset
                break;
            case BarType.Diagonal:
                offset = (sawBar.transform.right * (lengthX / 2)) + (sawBar.transform.up * (lengthY / 2));
                break;
        }
        
        Vector3 centerPosition = sawBar.transform.position;

        pointA = centerPosition - offset;
        pointB = centerPosition + offset;

        switch (startPoint)
        {
            case StartPoint.PointA:
                currentPoint = pointA;
                break;
            case StartPoint.PointB:
                currentPoint = pointB;
                break;
            default:
                currentPoint = pointA;
                break;
        }
    }

    private void FixedUpdate()
    {
        if (Vector3.Distance(saw.transform.position, currentPoint) < 0.1f)
        {
            currentPoint = pointA == currentPoint ? pointB : pointA;
        }

        saw.transform.position = Vector3.MoveTowards(saw.transform.position, currentPoint, moveSpeed * Time.deltaTime);
    }

}
