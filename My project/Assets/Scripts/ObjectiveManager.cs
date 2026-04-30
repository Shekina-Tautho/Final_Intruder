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
    private bool hasShownMacrophageWarning = false;

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

        // 🔴 Hide UI
        if (gameManager != null)
            gameManager.HideGameplayUI();

        titleText.text = "Mission Start: Influenza Virus";

        bodyText.text =
            "You are a viral particle entering human epithelial tissue.\n\n" +
            "Objective:\nInfect 10 cells.\n\n" +
            "Avoid macrophages that will attempt to destroy you.";
    }

    // ---------------- WARNING (ONLY ONCE) ----------------
    public void ShowMacrophageWarning(int wave)
    {
        if (hasShownMacrophageWarning) return;

        hasShownMacrophageWarning = true;

        isPanelActive = true;
        isIntroPanel = false;

        objectivePanel.SetActive(true);

        // ⛔ Pause timer
        if (gameTimer != null)
            gameTimer.timerActive = false;

        // 🔴 Hide UI
        if (gameManager != null)
            gameManager.HideGameplayUI();

        titleText.text = "Immune Response Increasing";

        bodyText.text =
            "Macrophages have been recruited to the infection site. These immune cells detect, engulf, and destroy infected or foreign particles.\n\n" +
            "What this means:\n" +
            "The body increases immune presence in infected tissue.";
    }

// ---------------- WIN PANEL ----------------
    public void ShowWinPanel()
    {
        isPanelActive = true;
        isIntroPanel = false;

        objectivePanel.SetActive(true);

        titleText.text = "Infection Successful";

         bodyText.text =
        "You have successfully infected a sufficient number of epithelial cells. Viral replication has overcome initial immune resistance in this region.\n\n" +
        "Infected cells now act as viral production sites, accelerating spread across surrounding tissue.\n\n" +
        "What happens next:\n" +
        "In real influenza cases, this level of infection typically triggers strong inflammatory responses, including fever, mucus production, and further immune escalation.";
    }

    // ---------------- LOSE PANEL ----------------
    public void ShowLosePanel()
    {
        isPanelActive = true;
        isIntroPanel = false;

        objectivePanel.SetActive(true);

        titleText.text = "Infection Contained";

        bodyText.text =
            "The host immune system has successfully limited viral spread. Macrophages and other immune defenses have eliminated infected cells before sufficient replication occurred.\n\n" +
            "Your infection was unable to reach a sustainable threshold for continued propagation.\n\n" +
            "What this means:\n" +
            "Early immune response is often capable of containing influenza before it spreads widely.";
    }

    // ---------------- CLOSE PANEL ----------------
    void HidePanel()
    {
        isPanelActive = false;
        objectivePanel.SetActive(false);

        // ▶ Resume timer
        if (gameTimer != null)
            gameTimer.timerActive = true;

        // ▶ Show UI again
        if (gameManager != null)
            gameManager.ShowGameplayUI();

        // ▶ Start game ONLY for intro
        if (isIntroPanel && gameManager != null)
        {
            gameManager.StartGame();
        }
    }

    // ---------------- INPUT ----------------
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