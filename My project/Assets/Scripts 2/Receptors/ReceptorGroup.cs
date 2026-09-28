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

                if (randomIncompatibleType == 0)
                    receptor.receptorType = "B";
                else
                    receptor.receptorType = "C";
            }

            Debug.Log(
                receptor.gameObject.name +
                " assigned receptor type: " +
                receptor.receptorType
            );
        }
    }

public void TryAttach(Virus virus)
{
    if (virus == null)
    {
        Debug.LogWarning("No virus was provided.");
        return;
    }

    Receptor closestReceptor = null;
    float closestDistance = Mathf.Infinity;

    foreach (Receptor receptor in receptors)
    {
        float distance = Vector3.Distance(
            virus.transform.position,
            receptor.transform.position
        );

        if (distance <= interactionRadius && distance < closestDistance)
        {
            closestDistance = distance;
            closestReceptor = receptor;
        }
    }

    if (closestReceptor == null)
    {
        Debug.Log("No receptor is within interaction range.");
        return;
    }

    Debug.Log(
        "Closest receptor: " +
        closestReceptor.gameObject.name +
        " | Distance: " +
        closestDistance
    );

    // Check if the virus matches the receptor
    if (virus.virusType == closestReceptor.receptorType)
    {
        Debug.Log(
            "MATCH! Virus type " +
            virus.virusType +
            " matched receptor type " +
            closestReceptor.receptorType
        );

        // Tell the virus to move toward the receptor
        virus.AttachToReceptor(closestReceptor.transform);

        Debug.Log(
            "RECEPTOR DETECTED - Virus moving toward receptor!"
        );
    }
    else
    {
        Debug.Log(
            "NO MATCH! Virus type " +
            virus.virusType +
            " does not match receptor type " +
            closestReceptor.receptorType
        );
    }
}
}