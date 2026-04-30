using UnityEngine;
using UnityEngine.SceneManagement;
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

    [Header("Scene Flow")]
    public string mainMenuScene = "MainMenu";

    [Header("Return Delay")]
    public float returnToMenuDelay = 5f;

    private bool hasWon = false;
    private bool hasLost = false;

    void Start()
    {
        // Show intro panel
        if (objectiveManager != null)
        {
            objectiveManager.ShowIntroObjective();
        }

        // Stop timer at start
        if (gameTimer != null)
        {
            gameTimer.timerActive = false;
        }

        // Hide gameplay UI at start
        HideGameplayUI();

        // Safety reset
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (playerStats == null) return;
        if (hasWon || hasLost) return;

        // WIN CONDITION
        if (playerStats.infectionCount >= winTarget)
        {
            WinGame();
            return;
        }

        // LOSE CONDITION
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

        // Stop timer
        if (gameTimer != null)
        {
            gameTimer.timerActive = false;
        }

        // Hide gameplay UI
        HideGameplayUI();

        // Infect all epithelial cells
        EpithelialInfectVisual[] cells = FindObjectsOfType<EpithelialInfectVisual>();

        float longestDelay = 0f;

        foreach (EpithelialInfectVisual cell in cells)
        {
            float delay = Random.Range(0f, 5f);

            cell.StartInfection(delay);

            if (delay > longestDelay)
            {
                longestDelay = delay;
            }
        }

        // Wait for infection animation
        StartCoroutine(ShowWinAfterInfection(longestDelay + 2f));
    }

    IEnumerator ShowWinAfterInfection(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        // Show panel
        if (objectiveManager != null)
        {
            objectiveManager.ShowWinPanel();
        }

        // Freeze game
        Time.timeScale = 0f;

        // Wait using realtime
        yield return new WaitForSecondsRealtime(returnToMenuDelay);

        // Resume before loading scene
        Time.timeScale = 1f;

        SceneManager.LoadScene(mainMenuScene);
    }

    // ---------------- LOSE ----------------
    void LoseGame()
    {
        hasLost = true;

        Debug.Log("LOSE → Infection Contained");

        // Stop timer
        if (gameTimer != null)
        {
            gameTimer.timerActive = false;
        }

        // Hide gameplay UI
        HideGameplayUI();

        // Slow motion effect
        Time.timeScale = 0.5f;

        // Show lose flow
        StartCoroutine(ShowLosePanelDelay());
    }

    IEnumerator ShowLosePanelDelay()
    {
        // Dramatic pause
        yield return new WaitForSecondsRealtime(1.5f);

        // Show lose panel
        if (objectiveManager != null)
        {
            objectiveManager.ShowLosePanel();
        }

        // Freeze game
        Time.timeScale = 0f;

        // Wait before returning to menu
        yield return new WaitForSecondsRealtime(returnToMenuDelay);

        // Restore time before changing scenes
        Time.timeScale = 1f;

        SceneManager.LoadScene(mainMenuScene);
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