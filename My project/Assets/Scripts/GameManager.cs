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

    [Header("Infection Feedback")]
    public AudioSource infectionAudioSource;
    public AudioClip squelchClip;
    public AudioClip pointClip;

    [Header("Win Audio")]
    public AudioClip winClip;

    [Header("Lose Audio")]
    public AudioClip loseClip;

    [Header("UI Feedback")]
    public TextMeshProUGUI infectionText;
    public Color normalColor = Color.white;
    public Color successColor = Color.yellow;

    private Coroutine infectionFlashRoutine;

    private bool hasWon = false;
    private bool hasLost = false;
    private bool waitingForPlayerExit = false;

    [Header("External Audio Controllers")]
    public MusicManager musicManager;
    public BackgroundHeartbeat heartbeatManager;

    void Start()
    {
        if (objectiveManager != null)
            objectiveManager.ShowIntroObjective();

        if (gameTimer != null)
            gameTimer.timerActive = false;

        HideGameplayUI();
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (playerStats == null) return;
        if (hasWon || hasLost) return;

        if (playerStats.infectionCount >= winTarget)
        {
            WinGame();
            return;
        }

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
            StopCoroutine(infectionFlashRoutine);

        infectionFlashRoutine = StartCoroutine(InfectionSequence());
    }

    IEnumerator InfectionSequence()
    {
        if (infectionAudioSource != null && squelchClip != null)
            infectionAudioSource.PlayOneShot(squelchClip);

        yield return new WaitForSeconds(0.18f);

        if (infectionAudioSource != null && pointClip != null)
            infectionAudioSource.PlayOneShot(pointClip);

        yield return new WaitForSeconds(0.05f);

        if (infectionText != null)
            yield return StartCoroutine(FlashInfectionText());
    }

    IEnumerator FlashInfectionText()
    {
        float duration = 0.25f;
        float timer = 0f;

        infectionText.color = successColor;

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
            gameTimer.timerActive = false;

        HideGameplayUI();

        // STOP MUSIC
        if (musicManager != null && musicManager.audioSource != null)
        {
            musicManager.audioSource.Stop();
        }

        // STOP HEARTBEAT
        if (heartbeatManager != null && heartbeatManager.audioSource != null)
        {
            heartbeatManager.audioSource.Stop();
            heartbeatManager.StopAllCoroutines();
        }

        // PLAY VISUAL EFFECTS
        EpithelialInfectVisual[] cells = FindObjectsOfType<EpithelialInfectVisual>();

        foreach (EpithelialInfectVisual cell in cells)
        {
            float delay = Random.Range(0f, 5f);
            cell.StartInfection(delay);
        }

        MacrophageDeath[] macrophages = FindObjectsOfType<MacrophageDeath>();

        foreach (MacrophageDeath m in macrophages)
        {
            if (m != null)
            {
                float delay = Random.Range(0f, 1.2f);
                m.TriggerDeath(delay);
            }
        }

        // START WIN SEQUENCE (NO DELAY)
        StartCoroutine(WinSequence());
    }

    IEnumerator WinSequence()
    {
        // PLAY WIN SOUND IMMEDIATELY
        if (infectionAudioSource != null && winClip != null)
        {
            infectionAudioSource.Stop();
            infectionAudioSource.clip = winClip;
            infectionAudioSource.Play();
        }

        // WAIT FOR SOUND TO FINISH
        if (winClip != null)
            yield return new WaitForSeconds(winClip.length);

        // SHOW WIN PANEL
        if (objectiveManager != null)
        {
            objectiveManager.ShowWinPanel();
        }

        // FREEZE GAME
        Time.timeScale = 0f;

        waitingForPlayerExit = true;
    }

    // ---------------- LOSE ----------------
    void LoseGame()
    {
        hasLost = true;

        Debug.Log("LOSE → Infection Contained");

        if (gameTimer != null)
            gameTimer.timerActive = false;

        HideGameplayUI();

        // ✅ STOP MUSIC
        if (musicManager != null && musicManager.audioSource != null)
        {
            musicManager.audioSource.Stop();
        }

        // ✅ STOP HEARTBEAT
        if (heartbeatManager != null && heartbeatManager.audioSource != null)
        {
            heartbeatManager.audioSource.Stop();
            heartbeatManager.StopAllCoroutines();
        }

        // slight slow motion before audio (optional but nice feel)
        Time.timeScale = 0.5f;

        StartCoroutine(LoseSequence());
    }

    IEnumerator LoseSequence()
    {
        // small dramatic delay (real-time so unaffected by timescale)
        yield return new WaitForSecondsRealtime(0.5f);

        // ✅ PLAY LOSE SOUND IMMEDIATELY
        if (infectionAudioSource != null && loseClip != null)
        {
            infectionAudioSource.Stop();
            infectionAudioSource.clip = loseClip;
            infectionAudioSource.Play();
        }

        // ✅ WAIT FULL AUDIO
        if (loseClip != null)
            yield return new WaitForSecondsRealtime(loseClip.length);

        // ✅ SHOW LOSE PANEL
        if (objectiveManager != null)
        {
            objectiveManager.ShowLosePanel();
        }

        // ✅ FREEZE GAME
        Time.timeScale = 0f;

        waitingForPlayerExit = true;
    }

    // ---------------- EXIT ----------------
    public void ExitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }

    // ---------------- START GAME ----------------
    public void StartGame()
    {
        Debug.Log("GAME STARTED");

        if (gameTimer != null)
            gameTimer.timerActive = true;

        ShowGameplayUI();
    }
}