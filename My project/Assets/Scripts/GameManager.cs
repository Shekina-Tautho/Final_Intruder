using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
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

    [Header("Infection Feedback")]
    public AudioSource infectionAudioSource;
    public AudioClip squelchClip;
    public AudioClip pointClip;

    [Header("UI Feedback")]
    public TextMeshProUGUI infectionText; // ✅ TEXT instead of Image
    public Color normalColor = Color.white;
    public Color successColor = Color.yellow;

    private Coroutine infectionFlashRoutine;

    private bool hasWon = false;
    private bool hasLost = false;

    void Start()
    {
        if (objectiveManager != null)
        {
            objectiveManager.ShowIntroObjective();
        }

        if (gameTimer != null)
        {
            gameTimer.timerActive = false;
        }

        HideGameplayUI();

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

    // ---------------- INFECTION FEEDBACK ----------------
    public void PlayInfectionFeedback()
    {
        if (infectionFlashRoutine != null)
        {
            StopCoroutine(infectionFlashRoutine);
        }

        infectionFlashRoutine = StartCoroutine(InfectionSequence());
    }

    IEnumerator InfectionSequence()
    {
        // 1. SQUELCH
        if (infectionAudioSource != null && squelchClip != null)
        {
            infectionAudioSource.PlayOneShot(squelchClip);
        }

        yield return new WaitForSeconds(0.18f);

        // 2. POINT SOUND
        if (infectionAudioSource != null && pointClip != null)
        {
            infectionAudioSource.PlayOneShot(pointClip);
        }

        // small delay so it "hits"
        yield return new WaitForSeconds(0.05f);

        // 3. TEXT COLOR FLASH
        if (infectionText != null)
        {
            yield return StartCoroutine(FlashInfectionText());
        }
    }

    IEnumerator FlashInfectionText()
    {
        float duration = 0.25f;
        float timer = 0f;

        // instant yellow
        infectionText.color = successColor;

        // fade back to white
        while (timer < duration)
        {
            timer += Time.deltaTime;

            infectionText.color = Color.Lerp(
                successColor,
                normalColor,
                timer / duration
            );

            yield return null;
        }

        infectionText.color = normalColor;
    }

    // ---------------- WIN ----------------
    void WinGame()
    {
        hasWon = true;

        Debug.Log("WIN → Epithelial infection spreading");

        if (gameTimer != null)
        {
            gameTimer.timerActive = false;
        }

        HideGameplayUI();

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

        StartCoroutine(ShowWinAfterInfection(longestDelay + 2f));
    }

    IEnumerator ShowWinAfterInfection(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (objectiveManager != null)
        {
            objectiveManager.ShowWinPanel();
        }

        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(returnToMenuDelay);

        Time.timeScale = 1f;

        SceneManager.LoadScene(mainMenuScene);
    }

    // ---------------- LOSE ----------------
    void LoseGame()
    {
        hasLost = true;

        Debug.Log("LOSE → Infection Contained");

        if (gameTimer != null)
        {
            gameTimer.timerActive = false;
        }

        HideGameplayUI();

        Time.timeScale = 0.5f;

        StartCoroutine(ShowLosePanelDelay());
    }

    IEnumerator ShowLosePanelDelay()
    {
        yield return new WaitForSecondsRealtime(1.5f);

        if (objectiveManager != null)
        {
            objectiveManager.ShowLosePanel();
        }

        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(returnToMenuDelay);

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