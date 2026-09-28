using UnityEngine;
using UnityEngine.InputSystem;

public class InfectionInputVR : MonoBehaviour
{
    [Header("VR Input")]
    public InputActionProperty infectAction;

    private Virus virus;

    void Start()
    {
        virus = GetComponent<Virus>();

        if (virus == null)
        {
            Debug.LogError(
                "InfectionInputVR could not find a Virus component on this GameObject."
            );
        }
    }

    void Update()
    {
        // PC testing
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryAttachToReceptor();
        }

        // VR Right Trigger
        float value = infectAction.action.ReadValue<float>();

        if (value > 0.8f)
        {
            TryAttachToReceptor();
        }
    }

    void TryAttachToReceptor()
    {
        if (virus == null)
        {
            Debug.LogWarning("No Virus component found.");
            return;
        }

        ReceptorGroup[] receptorGroups =
            FindObjectsOfType<ReceptorGroup>();

        if (receptorGroups.Length == 0)
        {
            Debug.LogWarning("No ReceptorGroup found in the scene.");
            return;
        }

        ReceptorGroup closestGroup = null;
        float closestDistance = Mathf.Infinity;

        foreach (ReceptorGroup group in receptorGroups)
        {
            float distance = Vector3.Distance(
                virus.transform.position,
                group.transform.position
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestGroup = group;
            }
        }

        if (closestGroup != null)
        {
            closestGroup.TryAttach(virus);
        }
    }
}