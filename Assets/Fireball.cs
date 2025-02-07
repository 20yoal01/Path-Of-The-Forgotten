using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] float speed = 20f;
    [SerializeField] float steer = 30f;
    [SerializeField] Transform target;

    Rigidbody2D rb;
    float input;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = transform.up * speed * Time.fixedDeltaTime * 10f;
        Vector2 direction = (target.position - transform.position).normalized;
        float rotationSteer = Vector3.Cross(transform.up, direction).z;
        rb.angularVelocity = rotationSteer * steer * 10f;

    }
}
