using UnityEngine;

public class ATPLightPulse : MonoBehaviour
{
    public Light glowLight;

    public float minIntensity = 1.5f;
    public float maxIntensity = 3.0f;
    public float pulseSpeed = 2f;

    void Update()
    {
        if (glowLight == null) return;

        float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
        glowLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, pulse);
    }
}