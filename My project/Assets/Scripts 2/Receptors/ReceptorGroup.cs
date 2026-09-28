using UnityEngine;

public class ReceptorGroup : MonoBehaviour
{
    [Header("Receptor Settings")]
    [Range(0f, 1f)]
    public float compatibleChance = 0.3f;

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
}