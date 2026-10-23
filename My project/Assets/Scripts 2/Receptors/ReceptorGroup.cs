using UnityEngine;

public class ReceptorGroup : MonoBehaviour
{
    [Header("Receptor Settings")]
    [Range(0f, 1f)]
    public float compatibleChance = 0.3f;

    [Header("Interaction Settings")]
    public float interactionRadius = 0.5f;

    private Receptor[] receptors;

    void Start()
    {
        receptors = GetComponentsInChildren<Receptor>();

        AssignReceptorTypes();

        Debug.Log("Found " + receptors.Length + " receptors.");
    }

    void AssignReceptorTypes()
    {
        foreach (Receptor receptor in receptors)
        {
            float randomValue = Random.value;

            if (randomValue <= compatibleChance)
            {
                receptor.receptorType = "A";
            }
            else
            {
                int randomIncompatibleType = Random.Range(0, 2);
                receptor.receptorType =
                    randomIncompatibleType == 0 ? "B" : "C";
            }

            Debug.Log(
                receptor.gameObject.name +
                " assigned receptor type: " +
                receptor.receptorType
            );
        }
    }

    // Existing method for NPCs.
    public void TryAttach(Virus virus)
    {
        TryAttach(virus, false);
    }

    // Player-triggered interaction.
    public bool TryAttach(Virus virus, bool playerInitiated)
    {
        if (virus == null)
        {
            Debug.LogWarning("No virus was provided.");
            return false;
        }

        Receptor closestReceptor = null;
        float closestDistance = Mathf.Infinity;

        foreach (Receptor receptor in receptors)
        {
            if (receptor == null)
                continue;

            float distance = Vector3.Distance(
                virus.transform.position,
                receptor.transform.position
            );

            if (distance <= interactionRadius &&
                distance < closestDistance)
            {
                closestDistance = distance;
                closestReceptor = receptor;
            }
        }

        if (closestReceptor == null)
            return false;

        Debug.Log(
            "Closest receptor: " +
            closestReceptor.gameObject.name +
            " | Distance: " + closestDistance
        );

        if (virus.virusType != closestReceptor.receptorType)
        {
            Debug.Log(
                "NO MATCH! Virus type " + virus.virusType +
                " does not match receptor type " +
                closestReceptor.receptorType
            );

            return true;
        }

        Debug.Log(
            "MATCH! Virus type " + virus.virusType +
            " matched receptor type " + closestReceptor.receptorType
        );

        closestReceptor.PlayMatchAnimation();

        Debug.Log("RECEPTOR DETECTED - Animation triggered!");

        if (playerInitiated)
        {
            PlayerCellSnapper snapper =
                FindFirstObjectByType<PlayerCellSnapper>();

            if (snapper != null)
                snapper.MoveToCell(closestReceptor);
            else
                Debug.LogWarning("No PlayerCellSnapper found in the scene.");
        }

        return true;
    }
}