
using UnityEngine;
using UnityEngine.InputSystem;

public class InfectionInputVR : MonoBehaviour
{
    [Header("VR Input")]
    public InputActionProperty infectAction;

    private Virus virus;
    private bool enabledActionHere = false;

    private void Awake()
    {
        virus = GetComponent<Virus>();

        if (virus == null)
        {
            Debug.LogError(
                "InfectionInputVR: No Virus component found!"
            );
        }
        else
        {
            Debug.Log(
                "Virus detected: " + virus.virusType
            );
        }
    }

    private void OnEnable()
    {
        if (infectAction.action == null)
        {
            Debug.LogError(
                "InfectionInputVR: Infect Action is NOT assigned!"
            );

            return;
        }

        // Enable the action if it is not already enabled.
        if (!infectAction.action.enabled)
        {
            infectAction.action.Enable();
            enabledActionHere = true;

            Debug.Log("Infect Action successfully enabled.");
        }
        else
        {
            Debug.Log("Infect Action is already enabled.");
        }

        // Display the actual input bindings.
        Debug.Log(
            "Active Input Action: " +
            infectAction.action.name
        );

        foreach (var control in infectAction.action.controls)
        {
            Debug.Log(
                "Infect Binding: " + control.path
            );
        }

        if (infectAction.action.controls.Count == 0)
        {
            Debug.LogWarning(
                "Infect Action has no resolved input controls!"
            );
        }
    }

    private void Update()
    {
        CheckKeyboardInput();
        CheckVRInput();
    }

    // ==========================================
    // KEYBOARD INPUT: E
    // ==========================================

    private void CheckKeyboardInput()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log(
                "KEYBOARD E DETECTED!"
            );

            TryAttachToReceptor();
        }
    }

    // ==========================================
    // VR INPUT: RIGHT CONTROLLER A BUTTON
    // ==========================================

    private void CheckVRInput()
    {
        if (infectAction.action == null)
            return;

        if (!infectAction.action.enabled)
            return;

        // Detect a new button press.
        if (infectAction.action.WasPressedThisFrame())
        {
            Debug.Log(
                "RIGHT CONTROLLER A BUTTON DETECTED!"
            );

            TryAttachToReceptor();
        }
    }

    // ==========================================
    // RECEPTOR ATTACHMENT
    // ==========================================

    private void TryAttachToReceptor()
    {
        if (virus == null)
        {
            Debug.LogWarning(
                "Cannot attach: Virus component is missing."
            );

            return;
        }

        ReceptorGroup[] receptorGroups =
            FindObjectsOfType<ReceptorGroup>();

        if (receptorGroups.Length == 0)
        {
            Debug.LogWarning(
                "No ReceptorGroup found in the scene."
            );

            return;
        }

        Debug.Log(
            "Checking " + receptorGroups.Length +
            " receptor group(s)..."
        );

        foreach (ReceptorGroup group in receptorGroups)
        {
            if (group == null)
                continue;

            bool interactionProcessed =
                group.TryAttach(virus, true);

            if (interactionProcessed)
            {
                Debug.Log(
                    "Receptor interaction processed successfully."
                );

                return;
            }
        }

        Debug.Log(
            "No receptor found within interaction range."
        );
    }

    // ==========================================
    // CLEANUP
    // ==========================================

    private void OnDisable()
    {
        // Only disable the action if this script enabled it.
        if (enabledActionHere && infectAction.action != null)
        {
            infectAction.action.Disable();

            enabledActionHere = false;
        }
    }
}
