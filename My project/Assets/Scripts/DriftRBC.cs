using UnityEngine;

public class DriftRBC : MonoBehaviour
{
    public float driftAmount = 0.2f;
    public float driftSpeed = 1f;

    private float randomOffsetX;
    private float randomOffsetY;

    void Start()
    {
        randomOffsetX = Random.Range(0f, 100f);
        randomOffsetY = Random.Range(0f, 100f);

        driftSpeed = Random.Range(0.5f, 1.5f);
    }

    void Update()
    {
        float x = Mathf.Sin(Time.time * driftSpeed + randomOffsetX) * driftAmount;
        float y = Mathf.Cos(Time.time * driftSpeed + randomOffsetY) * driftAmount;

        transform.position += new Vector3(x, y, 0) * Time.deltaTime;
    }
}