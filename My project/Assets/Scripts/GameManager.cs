using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerStats playerStats;
    public int winTarget = 10;

    private bool hasWon = false;

    void Update()
    {
        if (playerStats == null || hasWon) return;

        if (playerStats.infectionCount >= winTarget)
        {
            WinGame();
        }
    }

    void WinGame()
    {
        hasWon = true;
        Debug.Log("WIN → Epithelial infection spreading");

        // Get ALL epithelial cells
        EpithelialInfectVisual[] cells = FindObjectsOfType<EpithelialInfectVisual>();

        foreach (EpithelialInfectVisual cell in cells)
        {
            float delay = Random.Range(0f, 5f); // within 5 seconds
            cell.StartInfection(delay);
        }
    }
}