using UnityEngine;
using TMPro;

public class HUDInfectionDisplay : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI infectionText;

    [Header("Reference")]
    public PlayerStats playerStats;

    void Update()
    {
        if (playerStats == null || infectionText == null) return;

        infectionText.text = "" + playerStats.infectionCount;
    }
}