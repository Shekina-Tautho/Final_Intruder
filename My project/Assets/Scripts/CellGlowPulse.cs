using UnityEngine;

public class CellGlowPulse : MonoBehaviour
{
    private Renderer rend;
    private Material mat;
    private CellInfect cell;

    [Header("Healthy Glow (Purple)")]
    public Color emissionColor = new Color(0.4f, 0.1f, 0.8f);

    public float speed = 3.5f;
    public float baseIntensity = 0.15f;
    public float pulseStrength = 0.4f;

    [Header("Infected Glow (Red)")]
    public Color infectedColor = Color.red;
    public float infectedIntensity = 1.2f;

    void Start()
    {
        rend = GetComponentInChildren<Renderer>();
        mat = rend.material;

        cell = GetComponent<CellInfect>();

        mat.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        if (cell != null && cell.isInfected)
        {
            // 🔴 INFECTED STATE (no pulse)
            mat.SetColor("_EmissionColor", infectedColor * infectedIntensity);
            return;
        }

        // 🟣 HEALTHY PULSE STATE
        float t = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f;

        float intensity = baseIntensity + (t * pulseStrength);

        Color finalEmission = emissionColor * intensity;

        mat.SetColor("_EmissionColor", finalEmission);
    }
}