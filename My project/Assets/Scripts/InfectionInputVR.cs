
using UnityEngine;
using UnityEngine.InputSystem;

public class InfectionInputVR : MonoBehaviour
{
    [Header("VR Input")]
    public InputActionProperty infectAction;

    private Virus virus;
    private bool triggerWasPressed;

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
        // PC testing: E key
        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryAttachToReceptor();
        }

        // VR: Right Trigger
        if (infectAction.action == null)
            return;

        float value = infectAction.action.ReadValue<float>();

        bool triggerPressed = value > 0.8f;

        // Interact only when the trigger is first pressed.
        if (triggerPressed && !triggerWasPressed)
        {
            TryAttachToReceptor();
        }

        triggerWasPressed = triggerPressed;
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

        // Check each group instead of relying on parent positions.
        foreach (ReceptorGroup group in receptorGroups)
        {
            if (group != null && group.TryAttach(virus, true))
            {
                return;
            }
        }

        Debug.Log("No receptor was found within range in any group.");
    }
}
