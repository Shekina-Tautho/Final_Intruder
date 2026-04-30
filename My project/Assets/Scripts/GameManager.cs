using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public PlayerStats playerStats;
    public ObjectiveManager objectiveManager;

    [Header("Win Condition")]
    public int winTarget = 10;

    private bool hasWon = false;
    private bool hasLost = false;

    void Start()
    {
        // Show intro objective at game start
        if (objectiveManager != null)
        {
            objectiveManager.ShowIntroObjective();
        }
    }

    void Update()
    {
        if (playerStats == null) return;

        // Stop logic if game already ended
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

        // Optional: show win objective screen later
        if (objectiveManager != null)
        {
            // we will add ShowWin() later in next step
            // objectiveManager.ShowWin();
        }

        EpithelialInfectVisual[] cells = FindObjectsOfType<EpithelialInfectVisual>();

        foreach (EpithelialInfectVisual cell in cells)
        {
            float delay = Random.Range(0f, 5f);
            cell.StartInfection(delay);
        }
    }

    // ---------------- LOSE ----------------
    void LoseGame()
    {
        hasLost = true;

        Debug.Log("LOSE → Player died (life reached 0)");

        // Optional: show lose objective screen later
        if (objectiveManager != null)
        {
            // we will add ShowLose() later in next step
            // objectiveManager.ShowLose();
        }

        Time.timeScale = 0.5f;
    }
}