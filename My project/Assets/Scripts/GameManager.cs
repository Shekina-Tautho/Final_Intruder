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
            hasWon = true;
            Debug.Log("WIN CONDITION REACHED");
        }
    }
}