using UnityEngine;

public class CellInfect : MonoBehaviour
{
    public bool playerInRange = false;
    public bool isInfected = false;

    [Header("Visuals")]
    public Color healthyColor = new Color(0.4f, 0.1f, 0.8f);
    public Color infectedColor = new Color(0.7f, 0.05f, 0.05f); // deeper red, less pink

    [Header("Scale (VERY SUBTLE)")]
    public float inflamedScale = 0.08f; 
    public float growthSpeed = 0.8f;

    [Header("Pulse / Glow")]
    public float basePulseSpeed = 1.2f;
    public float pulseStrength = 0.08f;

    [Header("Glow Flicker")]
    public float flickerSpeed = 2f;
    public float flickerIntensity = 0.25f;

    private Vector3 originalScale;
    private Renderer rend;
    private Material mat;

    private float pulseOffset;
    private float noiseSeed;

    private float infectionProgress = 0f;

    void Start()
    {
        originalScale = transform.localScale;

        rend = GetComponentInChildren<Renderer>();
        mat = rend.material;

        mat.EnableKeyword("_EMISSION");

        pulseOffset = Random.Range(0f, 100f);
        noiseSeed = Random.Range(0f, 1000f);
    }

    void Update()
    {
        HandleVisuals();
    }

    // ---------------- PLAYER RANGE ----------------
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    // ---------------- INFECT ----------------
    public void TryInfect()
    {
        if (!playerInRange || isInfected) return;

        isInfected = true;

        PlayerStats stats = FindObjectOfType<PlayerStats>();
        if (stats != null)
            stats.infectionCount += 1;

        Debug.Log("Cell infected → subtle viral state");
    }

    // ---------------- VISUALS ----------------
    void HandleVisuals()
    {
        // 🧬 smooth infection progression
        if (isInfected)
        {
            infectionProgress += Time.deltaTime * 0.25f;
            infectionProgress = Mathf.Clamp01(infectionProgress);
        }

        // ---------------- SCALE (VERY SUBTLE) ----------------
        float scaleFactor = Mathf.Lerp(1f, inflamedScale, infectionProgress);
        Vector3 targetScale = originalScale * scaleFactor;

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.deltaTime * growthSpeed
        );

        // ---------------- BASE PULSE (organic breathing) ----------------
        float pulse = Mathf.Sin((Time.time + pulseOffset) * basePulseSpeed);
        pulse = (pulse + 1f) * 0.5f; // normalize 0–1

        // ---------------- RANDOM FLICKER (key improvement) ----------------
        float flickerNoise = Mathf.PerlinNoise(Time.time * flickerSpeed, noiseSeed);
        flickerNoise = (flickerNoise - 0.5f) * flickerIntensity;

        // ---------------- INFECTION INTENSITY ----------------
        float infectionGlow = Mathf.Lerp(0.1f, 0.9f, infectionProgress);

        float finalIntensity =
            (pulse * pulseStrength) +
            flickerNoise +
            infectionGlow;

        // ---------------- COLOR SHIFT ----------------
        Color baseColor = Color.Lerp(healthyColor, infectedColor, infectionProgress);

        // 🔥 emission stays primary feedback (NO overbrightening)
        Color emission = baseColor * finalIntensity;

        mat.SetColor("_EmissionColor", emission);
    }
}