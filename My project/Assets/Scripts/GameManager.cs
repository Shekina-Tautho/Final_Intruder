using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public PlayerStats playerStats;
    public ObjectiveManager objectiveManager;

    [Header("Game Flow References")]
    public GameTimer gameTimer;

    [Header("UI Elements")]
    public GameObject timerUI;
    public GameObject infectionUI;
    public GameObject staminaUI;
    public GameObject lifeUI;
    public GameObject outlineUI;

    [Header("Win Condition")]
    public int winTarget = 10;

    private bool hasWon = false;
    private bool hasLost = false;

    void Start()
    {
        // 🔴 LOCK GAMEPLAY AT START

        if (objectiveManager != null)
        {
            objectiveManager.ShowIntroObjective();
        }

        if (gameTimer != null)
        {
            gameTimer.timerActive = false;
        }
        

        // Hide ALL UI at start
        if (timerUI != null) timerUI.SetActive(false);
        if (infectionUI != null) infectionUI.SetActive(false);
        if (staminaUI != null) staminaUI.SetActive(false);
        if (lifeUI != null) lifeUI.SetActive(false);
        if (outlineUI != null) outlineUI.SetActive(false);
    }

    void Update()
    {
        if (playerStats == null) return;
        if (hasWon || hasLost) return;

        // ✅ WIN CONDITION
        if (playerStats.infectionCount >= winTarget)
        {
            WinGame();
            return;
        }

        // ✅ LOSE CONDITION
        if (playerStats.currentLife <= 0f)
        {
            LoseGame();
            return;
        }
    }

    // ---------------- WIN ----------------
    void WinGame()
    {
        hasWon = true;

        Debug.Log("WIN → Epithelial infection spreading");

        if (objectiveManager != null)
        {
            // objectiveManager.ShowWin(); (future step)
        }

        EpithelialInfectVisual[] cells = FindObjectsOfType<EpithelialInfectVisual>();

        foreach (EpithelialInfectVisual cell in cells)
        {
            float delay = UnityEngine.Random.Range(0f, 5f);
            cell.StartInfection(delay);
        }
    }

    // ---------------- LOSE ----------------
    void LoseGame()
    {
        hasLost = true;

        Debug.Log("LOSE → Player died (life reached 0)");

        if (objectiveManager != null)
        {
            // objectiveManager.ShowLose(); (future step)
        }

        Time.timeScale = 0.5f;
    }

    // ---------------- GAME START ----------------
    public void StartGame()
    {
        Debug.Log("GAME STARTED");

        if (gameTimer != null)
        {
            gameTimer.timerActive = true;
        }

        if (timerUI != null) timerUI.SetActive(true);
        if (infectionUI != null) infectionUI.SetActive(true);
        if (staminaUI != null) staminaUI.SetActive(true);
        if (lifeUI != null) lifeUI.SetActive(true);
        if (outlineUI != null) outlineUI.SetActive(true);
    }
}