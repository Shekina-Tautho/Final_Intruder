using UnityEngine;

public class BacteriaFlow : MonoBehaviour
{
    [Header("Flow")]
    public Vector3 baseDirection = new Vector3(0, 0, -1);
    public float resetDistance = 50f;

    [Header("Speed")]
    public float minSpeed = 0.5f;
    public float maxSpeed = 2.5f;

    [Header("Drift")]
    public float driftAmount = 0.2f;
    public float driftSpeed = 1f;

    private float flowSpeed;
    private Vector3 direction;
    private Vector3 startPos;

    private float noiseOffsetX;
    private float noiseOffsetY;

    private Vector3 rotationAxis;
    private float rotationSpeed;

    void Start()
    {
        startPos = transform.position;

        // RANDOMIZE SPEED
        flowSpeed = Random.Range(minSpeed, maxSpeed);

        // SLIGHT DIRECTION VARIATION
        direction = baseDirection + new Vector3(
            Random.Range(-0.3f, 0.3f),
            Random.Range(-0.3f, 0.3f),
            0
        );
        direction.Normalize();

        // ROTATION
        rotationAxis = Random.insideUnitSphere;
        rotationSpeed = Random.Range(10f, 60f);

        // DRIFT NOISE
        noiseOffsetX = Random.Range(0f, 1000f);
        noiseOffsetY = Random.Range(0f, 1000f);

        // RANDOM DRIFT SPEED PER BACTERIA
        driftSpeed = Random.Range(0.3f, 1.8f);
    }

    void Update()
    {
        // MAIN FLOW (blood/tissue movement)
        transform.position += direction * flowSpeed * Time.deltaTime;

        // RESET LOOP
        if (Vector3.Distance(startPos, transform.position) > resetDistance)
        {
            transform.position = startPos + Random.insideUnitSphere * 2f;
        }

        // ROTATION
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);

        // ORGANIC DRIFT (noise-based)
        float x = Mathf.Sin(Time.time * driftSpeed + noiseOffsetX) * driftAmount;
        float y = Mathf.Cos(Time.time * driftSpeed + noiseOffsetY) * driftAmount;

        transform.position += new Vector3(x, y, 0) * Time.deltaTime;
    }
}