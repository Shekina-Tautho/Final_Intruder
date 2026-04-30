using UnityEngine;
using System.Collections;

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
        // 🔴 Show intro panel
        if (objectiveManager != null)
        {
            objectiveManager.ShowIntroObjective();
        }

        // 🔴 Stop timer at start
        if (gameTimer != null)
        {
            gameTimer.timerActive = false;
        }

        // 🔴 Hide ALL gameplay UI
        HideGameplayUI();
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

    // ---------------- UI CONTROL ----------------
    public void ShowGameplayUI()
    {
        if (timerUI != null) timerUI.SetActive(true);
        if (infectionUI != null) infectionUI.SetActive(true);
        if (staminaUI != null) staminaUI.SetActive(true);
        if (lifeUI != null) lifeUI.SetActive(true);
        if (outlineUI != null) outlineUI.SetActive(true);
    }

    public void HideGameplayUI()
    {
        if (timerUI != null) timerUI.SetActive(false);
        if (infectionUI != null) infectionUI.SetActive(false);
        if (staminaUI != null) staminaUI.SetActive(false);
        if (lifeUI != null) lifeUI.SetActive(false);
        if (outlineUI != null) outlineUI.SetActive(false);
    }

    // ---------------- WIN ----------------
    void WinGame()
    {
        hasWon = true;

        Debug.Log("WIN → Epithelial infection spreading");

        // STOP TIMER
        if (gameTimer != null)
        {
            gameTimer.timerActive = false;
        }

        // HIDE GAMEPLAY UI
        HideGameplayUI();

        EpithelialInfectVisual[] cells = FindObjectsOfType<EpithelialInfectVisual>();

        float longestDelay = 0f;

        foreach (EpithelialInfectVisual cell in cells)
        {
            float delay = UnityEngine.Random.Range(0f, 5f);

            cell.StartInfection(delay);

            if (delay > longestDelay)
                longestDelay = delay;
        }

        // WAIT until infection animation completes
        StartCoroutine(ShowWinAfterInfection(longestDelay + 2f));
    }

    IEnumerator ShowWinAfterInfection(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (objectiveManager != null)
        {
            objectiveManager.ShowWinPanel();
        }

        // FREEZE GAME
        Time.timeScale = 0f;
    }

    IEnumerator ShowLosePanelDelay()
    {
        yield return new WaitForSecondsRealtime(1.5f);

        if (objectiveManager != null)
        {
            objectiveManager.ShowLosePanel();
        }

        // FREEZE GAME
        Time.timeScale = 0f;
    }

    // ---------------- LOSE ----------------
    void LoseGame()
    {
        hasLost = true;

        Debug.Log("LOSE → Infection Contained");

        // STOP TIMER
        if (gameTimer != null)
        {
            gameTimer.timerActive = false;
        }

        // HIDE GAMEPLAY UI
        HideGameplayUI();

        // OPTIONAL slow motion effect
        Time.timeScale = 0.5f;

        // SHOW PANEL AFTER SHORT DELAY
        StartCoroutine(ShowLosePanelDelay());
    }
    // ---------------- GAME START ----------------
    public void StartGame()
    {
        Debug.Log("GAME STARTED");

        if (gameTimer != null)
        {
            gameTimer.timerActive = true;
        }

        ShowGameplayUI();
    }
}