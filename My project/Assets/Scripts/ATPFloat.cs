using UnityEngine;

public class ATPFloat : MonoBehaviour
{
    public float amplitude = 0.08f;
    public float frequency = 2f;
    public float rotationSpeed = 30f;

    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // floating up/down
        transform.position = startPos +
            Vector3.up * Mathf.Sin(Time.time * frequency) * amplitude;

        // slow rotation
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}