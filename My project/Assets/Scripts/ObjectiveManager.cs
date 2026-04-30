using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ObjectiveManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject objectivePanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;

    private bool isPanelActive = false;

    void Update()
    {
        if (isPanelActive)
        {
            if (ShouldClosePanel())
            {
                HidePanel();
            }
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
        // Keyboard
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
            return true;

        // Mouse
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;

        // VR placeholder
        if (CheckVRInput())
            return true;

        return false;
    }

    bool CheckVRInput()
    {
        // Future XR integration goes here
        return false;
    }
}