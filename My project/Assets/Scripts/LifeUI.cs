using UnityEngine;
using UnityEngine.UI;

public class LifeUI : MonoBehaviour
{
    public PlayerStats playerStats;
    public Image lifeBar;

    void Update()
    {
        if (playerStats == null || lifeBar == null) return;

        lifeBar.fillAmount = playerStats.currentLife / playerStats.maxLife;
        Debug.Log("UI reading life: " + playerStats.currentLife);
    }
}