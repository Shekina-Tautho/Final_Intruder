using UnityEngine;

public class CellInfect : MonoBehaviour
{
    public bool playerInRange = false;
    public bool isInfected = false;

    public float inflamedScale = 1.3f;
    private Vector3 originalScale;

    private Renderer rend;

    public Color infectedEmission = new Color(0.6f, 0.1f, 0.9f);

    void Start()
    {
        originalScale = transform.localScale;
        rend = GetComponentInChildren<Renderer>();
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

        // increase infection count
        PlayerStats stats = FindObjectOfType<PlayerStats>();
        if (stats != null)
        {
            stats.infectionCount += 1;
        }

        // inflate cell
        transform.localScale = originalScale * inflamedScale;

        // change glow
        if (rend != null)
        {
            rend.material.SetColor("_EmissionColor", infectedEmission * 0.8f);
        }

        Debug.Log("Cell infected via proximity!");
    }
}