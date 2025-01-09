using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    private Camera cam;
    private Transform followTarget;

    Vector2 startingPosition;
    Vector2 camMoveSinceStart => (Vector2)cam.transform.position - startingPosition;

    float zDistanceFromTarget => transform.transform.position.z - followTarget.transform.position.z;
    float clippingPlane => (cam.transform.position.z + (zDistanceFromTarget > 0 ? cam.farClipPlane : cam.nearClipPlane));

    float parallaxFactor => Mathf.Abs(zDistanceFromTarget) / clippingPlane;

    float startingZ;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        followTarget = GameObject.FindGameObjectWithTag("Player").transform;

        if (cam == null)
        {
            Debug.LogError("MainCamera with tag 'MainCamera' not found!");
            return;
        }

        if (followTarget == null)
        {
            Debug.LogError("Player with tag 'Player' not found!");
            return;
        }

        startingPosition = transform.position;
        startingZ = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (cam == null || followTarget == null)
            return;

        Vector2 newPostion = startingPosition + camMoveSinceStart * parallaxFactor;

        transform.position = new Vector3(newPostion.x, newPostion.y, startingZ);
    }
}