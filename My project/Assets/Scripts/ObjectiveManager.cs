using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ObjectiveManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject objectivePanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;

    [Header("Game References")]
    public GameManager gameManager;

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
            "You are a viral particle entering human epithelial tissue. Your goal is to infect host cells and spread by targeting vulnerable tissue clusters. Macrophages and other immune cells patrol this environment to detect and eliminate pathogens.\n\n" +
            "Primary Objective:\n" +
            "Infect epithelial cells and reach the required infection count of 10 to establish successful viral spread.\n\n" +
            "Threat:\n" +
            "In real influenza infections, viruses target respiratory epithelial cells to replicate.";
    }

    // ---------------- CLOSE ----------------
    void HidePanel()
    {
        isPanelActive = false;
        objectivePanel.SetActive(false);

        // 🟢 START GAME FLOW HERE
        if (gameManager != null)
        {
            gameManager.StartGame();
        }
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

        // VR / XR Input Action
        if (closePanelAction != null && closePanelAction.action.WasPressedThisFrame())
            return true;

        return false;
    }
}