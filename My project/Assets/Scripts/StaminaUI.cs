using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    public PlayerStats playerStats;
    public Image staminaBar;

    void Update()
    {
        if (playerStats == null || staminaBar == null) return;

        staminaBar.fillAmount = playerStats.currentStamina / playerStats.maxStamina;
    }
}