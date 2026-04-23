using UnityEngine;

public class CellInfect : MonoBehaviour
{
    public bool playerInRange = false;
    public bool isInfected = false;

    [Header("Visuals")]
    public Color infectedEmission = Color.red;
    public float inflamedScale = 1.3f;

    private Vector3 originalScale;
    private Renderer rend;
    private Material mat;

    void Start()
    {
        originalScale = transform.localScale;
        rend = GetComponentInChildren<Renderer>();
        mat = rend.material;

        mat.EnableKeyword("_EMISSION");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void TryInfect()
    {
        if (!playerInRange || isInfected) return;

        Infect();
    }

    void Infect()
    {
        isInfected = true;

        // 🧠 increase infection count
        PlayerStats stats = FindObjectOfType<PlayerStats>();
        if (stats != null)
        {
            stats.infectionCount += 1;
        }

        // 📈 inflate cell
        transform.localScale = originalScale * inflamedScale;

        // 🔴 FORCE RED EMISSION (infected state)
        mat.SetColor("_EmissionColor", infectedEmission * 1.2f);

        Debug.Log("Cell infected → RED STATE");
    }
}