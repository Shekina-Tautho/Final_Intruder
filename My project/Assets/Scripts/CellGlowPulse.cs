using UnityEngine;

public class CellGlowPulse : MonoBehaviour
{
    private Renderer rend;
    private Material mat;
    private CellInfect cell;

    [Header("Healthy Glow (Purple)")]
    public Color emissionColor = new Color(0.4f, 0.1f, 0.8f);

    public float baseIntensity = 0.15f;
    public float pulseStrength = 0.4f;

    [Header("Infected Glow (Red)")]
    public Color infectedColor = Color.red;
    public float infectedIntensity = 1.2f;

    // 🔥 NEW VARIATION SYSTEM
    private float phaseOffset;
    private float speed;
    private float drift;

    // each cell has its own internal time
    private float localTime;

    void Start()
    {
        rend = GetComponentInChildren<Renderer>();
        mat = rend.material;

        cell = GetComponent<CellInfect>();

        mat.EnableKeyword("_EMISSION");

        // 🔥 CORE RANDOMIZATION (this is key)
        phaseOffset = Random.Range(0f, 100f);
        speed = Random.Range(2.0f, 5.0f);

        // 🔥 slow time drift makes each cell feel “independent”
        drift = Random.Range(0.8f, 1.2f);

        localTime = Random.Range(0f, 100f);
    }

    void Update()
    {
        if (cell != null && cell.isInfected)
        {
            mat.SetColor("_EmissionColor", infectedColor * infectedIntensity);
            return;
        }

        // 🔥 each cell runs its OWN time instead of global time
        localTime += Time.deltaTime * drift;

        float wave = Mathf.Sin((localTime * speed) + phaseOffset);

        float t = (wave + 1f) * 0.5f;

        // 🔥 per-cell intensity variation (breaks uniform brightness)
        float intensityVariation = Random.Range(0.85f, 1.15f);

        float intensity = (baseIntensity + (t * pulseStrength)) * intensityVariation;

        Color finalEmission = emissionColor * intensity;

        mat.SetColor("_EmissionColor", finalEmission);
    }
}