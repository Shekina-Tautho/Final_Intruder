using UnityEngine;

public class FlowRBC : MonoBehaviour
{
    public Vector3 flowDirection = new Vector3(0, 0, -1);
    public float flowSpeed = 0.5f;
    public float resetDistance = 50f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        transform.position += flowDirection * flowSpeed * Time.deltaTime;

        if (Vector3.Distance(startPos, transform.position) > resetDistance)
        {
            transform.position = startPos;
        }

        transform.Rotate(Vector3.up * 20f * Time.deltaTime);
    }
}