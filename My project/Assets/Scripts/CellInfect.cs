using UnityEngine;

public class CellInfect : MonoBehaviour
{
    public bool playerInRange = false;
    public bool isInfected = false;

    [Header("Visuals")]
    public Color healthyColor = new Color(0.4f, 0.1f, 0.8f);
    public Color infectedColor = new Color(0.7f, 0.05f, 0.05f);

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

    private GameManager gameManager;

    void Start()
    {
        originalScale = transform.localScale;

        rend = GetComponentInChildren<Renderer>();
        mat = rend.material;

        mat.EnableKeyword("_EMISSION");

        pulseOffset = Random.Range(0f, 100f);
        noiseSeed = Random.Range(0f, 1000f);

        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        HandleVisuals();
    }

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

    public void TryInfect()
    {
        if (!playerInRange || isInfected) return;

        isInfected = true;

        PlayerStats stats = FindObjectOfType<PlayerStats>();

        if (stats != null)
        {
            stats.infectionCount += 1;
        }

        // FEEDBACK (sound + UI)
        if (gameManager != null)
        {
            gameManager.PlayInfectionFeedback();
        }

        Debug.Log("Cell infected → subtle viral state");
    }

    void HandleVisuals()
    {
        if (isInfected)
        {
            infectionProgress += Time.deltaTime * 0.25f;
            infectionProgress = Mathf.Clamp01(infectionProgress);
        }

        float scaleFactor = Mathf.Lerp(1f, inflamedScale, infectionProgress);
        Vector3 targetScale = originalScale * scaleFactor;

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.deltaTime * growthSpeed
        );

        float pulse = Mathf.Sin((Time.time + pulseOffset) * basePulseSpeed);
        pulse = (pulse + 1f) * 0.5f;

        float flickerNoise = Mathf.PerlinNoise(Time.time * flickerSpeed, noiseSeed);
        flickerNoise = (flickerNoise - 0.5f) * flickerIntensity;

        float infectionGlow = Mathf.Lerp(0.1f, 0.9f, infectionProgress);

        float finalIntensity =
            (pulse * pulseStrength) +
            flickerNoise +
            infectionGlow;

        Color baseColor = Color.Lerp(healthyColor, infectedColor, infectionProgress);
        Color emission = baseColor * finalIntensity;

        mat.SetColor("_EmissionColor", emission);
    }
}