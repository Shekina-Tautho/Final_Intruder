using UnityEngine;

public class DriftRBC : MonoBehaviour
{
    public float driftAmount = 0.2f;
    public float driftSpeed = 1f;

    private Vector3 offset;

    void Start()
    {
        offset = Random.insideUnitSphere;
    }

    void Update()
    {
        float x = Mathf.Sin(Time.time * driftSpeed + offset.x) * driftAmount;
        float y = Mathf.Cos(Time.time * driftSpeed + offset.y) * driftAmount;

        transform.position += new Vector3(x, y, 0) * Time.deltaTime;
    }
}