using UnityEngine;

public class ATPBloomPulse : MonoBehaviour
{
    public float pulseSpeed = 3f;
    public float pulseAmount = 0.15f;

    Vector3 baseScale;

    void Start()
    {
        baseScale = transform.localScale;
    }

    void Update()
    {
        float pulse = 1 + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = baseScale * pulse;
    }
}