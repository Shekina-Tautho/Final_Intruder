using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerStats playerStats;
    public int winTarget = 10;

    private bool hasWon = false;
    private bool hasLost = false;

    void Update()
    {
        if (playerStats == null) return;

        // Stop if game already ended
        if (hasWon || hasLost) return;

        // ✅ WIN CONDITION
        if (playerStats.infectionCount >= winTarget)
        {
            WinGame();
            return;
        }

        // ✅ LOSE CONDITION (LIFE BASED)
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

        // Optional: slow motion effect
        Time.timeScale = 0.5f;
    }
}