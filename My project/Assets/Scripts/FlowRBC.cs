using UnityEngine;

public class FlowRBC : MonoBehaviour
{
    public Vector3 baseDirection = new Vector3(0, 0, -1);

    private float flowSpeed;
    private Vector3 direction;

    public float resetDistance = 50f;
    private Vector3 startPos;
    private Vector3 rotationAxis;
    private float rotationSpeed;

    void Start()
    {
        startPos = transform.position;

        // RANDOM SPEED
        flowSpeed = Random.Range(1.0f, 2.5f);

        // RANDOM SLIGHT DIRECTION OFFSET
        direction = baseDirection + new Vector3(
            Random.Range(-0.2f, 0.2f),
            Random.Range(-0.2f, 0.2f),
            0
        );

        direction.Normalize();

        rotationAxis = Random.insideUnitSphere; // random rotation axis
        rotationSpeed = Random.Range(10f, 40f); // random speed

    }

    void Update()
    {
        transform.position += direction * flowSpeed * Time.deltaTime;

        if (Vector3.Distance(startPos, transform.position) > resetDistance)
        {
            transform.position = startPos;
        }
        
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
    }
}