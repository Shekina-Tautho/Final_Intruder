using UnityEngine;
using System.Collections;

public class EpithelialInfectVisual : MonoBehaviour
{
    [Header("Visuals")]
    public Color healthyColor = new Color(0.4f, 0.1f, 0.8f);
    public Color infectedColor = new Color(0.7f, 0.05f, 0.05f);

    [Header("Glow Settings")]
    public float glowStrength = 1.5f;
    public float transitionSpeed = 1.5f;

    private Renderer rend;
    private Material mat;

    private float infectionProgress = 0f;
    private bool startInfecting = false;

    void Start()
    {
        rend = GetComponentInChildren<Renderer>();

        if (rend != null)
        {
            mat = rend.material;
            mat.EnableKeyword("_EMISSION");
        }
    }

    void Update()
    {
        if (!startInfecting || mat == null) return;

        // Smooth transition
        infectionProgress += Time.deltaTime * transitionSpeed;
        infectionProgress = Mathf.Clamp01(infectionProgress);

        Color baseColor = Color.Lerp(healthyColor, infectedColor, infectionProgress);
        Color emission = baseColor * glowStrength;

        mat.SetColor("_EmissionColor", emission);
    }

    // 🔥 CALLED BY GAME MANAGER
    public void StartInfection(float delay)
    {
        StartCoroutine(DelayedStart(delay));
    }

    IEnumerator DelayedStart(float delay)
    {
        yield return new WaitForSeconds(delay);
        startInfecting = true;
    }
}