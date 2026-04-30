using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ObjectiveManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject objectivePanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;

    [Header("Input (VR / New Input System)")]
    public InputActionReference closePanelAction;

    private bool isPanelActive = false;

    // ---------------- LIFECYCLE ----------------
    void OnEnable()
    {
        if (closePanelAction != null)
            closePanelAction.action.Enable();
    }

    void OnDisable()
    {
        if (closePanelAction != null)
            closePanelAction.action.Disable();
    }

    void Update()
    {
        if (isPanelActive && ShouldClosePanel())
        {
            HidePanel();
        }
    }

    // ---------------- INTRO ----------------
    public void ShowIntroObjective()
    {
        isPanelActive = true;
        objectivePanel.SetActive(true);

        titleText.text = "Mission Start: Influenza Virus";

        bodyText.text =
            "You are an Influenza Virus entering a human host.\n\n" +
            "Primary Objective:\n" +
            "- Infect target cells to increase infection count\n\n" +
            "Threat:\n" +
            "- Macrophages patrol and eliminate viruses\n\n" +
            "Goal: Spread before immune system destroys you.";
    }

    // ---------------- CLOSE ----------------
    void HidePanel()
    {
        isPanelActive = false;
        objectivePanel.SetActive(false);
    }

    // ---------------- INPUT ----------------
    bool ShouldClosePanel()
    {
        // Keyboard (any key)
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
            return true;

        // Mouse click (left button)
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;

        // VR / XR Input Action (REAL FIX)
        if (closePanelAction != null && closePanelAction.action.WasPressedThisFrame())
            return true;

        return false;
    }
}