using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ObjectiveManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject objectivePanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI bodyText;

    [Header("References")]
    public GameManager gameManager;
    public GameTimer gameTimer;

    [Header("Input")]
    public InputActionReference closePanelAction;

    private bool isPanelActive = false;
    private bool isIntroPanel = false;

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

    void Start()
    {
        if (gameTimer != null)
        {
            gameTimer.OnMacrophageWaveSpawn += ShowMacrophageWarning;
        }
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
        isIntroPanel = true;

        objectivePanel.SetActive(true);

        titleText.text = "Mission Start: Influenza Virus";

        bodyText.text =
            "You are a viral particle entering human epithelial tissue.\n\n" +
            "Objective:\nInfect 10 cells.\n\n" +
            "Avoid macrophages that will attempt to destroy you.";
    }

    // ---------------- WARNING ----------------
    public void ShowMacrophageWarning(int wave)
    {
        isPanelActive = true;
        isIntroPanel = false;

        objectivePanel.SetActive(true);

        if (gameTimer != null)
            gameTimer.timerActive = false;

        titleText.text = "Immune Response Increasing";

        bodyText.text =
            "Macrophages have been recruited to the infection site. These immune cells detect, engulf, and destroy infected or foreign particles.\n\n" +
            "What this means:\n" +
            "The body increases immune presence in infected tissue.\n\n" +
            "Wave: " + wave;
    }

    // ---------------- CLOSE ----------------
    void HidePanel()
    {
        isPanelActive = false;
        objectivePanel.SetActive(false);

        // Resume timer
        if (gameTimer != null)
            gameTimer.timerActive = true;

        // ONLY start game once (intro)
        if (isIntroPanel && gameManager != null)
        {
            gameManager.StartGame();
        }
    }

    bool ShouldClosePanel()
    {
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
            return true;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;

        if (closePanelAction != null && closePanelAction.action.WasPressedThisFrame())
            return true;

        return false;
    }
}