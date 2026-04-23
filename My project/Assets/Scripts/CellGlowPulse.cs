using UnityEngine;

public class CellGlowPulse : MonoBehaviour
{
    private Renderer rend;
    private Material mat;

    public Color emissionColor = new Color(0.4f, 0.1f, 0.8f);

    public float speed = 3.5f;          // 🔥 faster pulse
    public float baseIntensity = 0.15f; // 🔥 resting glow (very low)
    public float pulseStrength = 0.4f;  // 🔥 variation amount

    void Start()
    {
        rend = GetComponentInChildren<Renderer>();
        mat = rend.material;

        mat.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        float t = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f;

        // 🔥 true breathing curve (not always-on brightness)
        float intensity = baseIntensity + (t * pulseStrength);

        Color finalEmission = emissionColor * intensity;

        mat.SetColor("_EmissionColor", finalEmission);
    }
}