using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverAnimation : MonoBehaviour
{
    public float amp;
    public float freq;
    Vector3 initPos;

    // Update is called once per frame
    private void Start()
    {
        initPos = transform.position;
    }

    void Update()
    {
        transform.position = new Vector3(initPos.x, initPos.y + (Mathf.Sin(Time.time * freq) * amp), initPos.z); 
    }
}
