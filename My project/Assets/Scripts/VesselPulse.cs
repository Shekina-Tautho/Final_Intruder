using UnityEngine;

public class VesselPulse : MonoBehaviour
{
    public float speed = 2f;
    public float scaleAmount = 0.02f;

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        float scale = 1 + Mathf.Sin(Time.time * speed) * scaleAmount;
        transform.localScale = new Vector3(
            originalScale.x * scale,
            originalScale.y,
            originalScale.z * scale
        );
    }
}