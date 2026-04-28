using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public float startTime = 60f;
    private float currentTime;

    [Header("UI")]
    public TextMeshProUGUI timerText;

    void Start()
    {
        currentTime = startTime;
    }

    void Update()
    {
        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = startTime; // reset (Option A)
        }

        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        int seconds = Mathf.CeilToInt(currentTime);
        timerText.text = seconds.ToString();
    }
}