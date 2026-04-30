using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class GameTimer : MonoBehaviour
{
    [Header("Timer")]
    public float startTime = 60f;
    private float currentTime;

    public bool timerActive = false;

    private bool dangerMode = false;

    [Header("UI")]
    public TextMeshProUGUI timerText;

    [Header("VR Screen Effects")]
    public Image vignetteImage;
    public Image flashImage;

    private float flashValue;

    [Header("Audio Warning")]
    public AudioSource beepSource;
    public float beepInterval = 1f;
    private float beepTimer;

    [Header("Spawn Sound")]
    public AudioSource spawnWaveSource;

    [Header("Spawning")]
    public GameObject macrophagePrefab;
    public Transform[] spawnPoints;

    [System.Serializable]
    public class PathGroup
    {
        public Transform[] points;
    }

    public PathGroup[] paths;

    [Header("Difficulty")]
    public int baseSpawnCount = 2;
    private int currentWave = 1;

    // ✅ EVENT
    public Action<int> OnMacrophageWaveSpawn;

    void Start()
    {
        currentTime = startTime;
        beepTimer = 0f;
    }

    void Update()
    {
        if (!timerActive) return;

        currentTime -= Time.deltaTime;

        HandleDangerMode();
        HandleBeepWarning();
        HandleScreenEffects();

        if (currentTime <= 0f)
        {
            SpawnWave();
            currentWave++;
            currentTime = startTime;
            beepTimer = 0f;
        }

        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        int seconds = Mathf.CeilToInt(currentTime);
        timerText.text = seconds.ToString();
    }

    void HandleDangerMode()
    {
        timerText.color = (currentTime <= 5f) ? Color.red : Color.white;
    }

    void HandleBeepWarning()
    {
        if (currentTime <= 5f)
        {
            float normalized = currentTime / 5f;
            float interval = Mathf.Lerp(0.1f, beepInterval, normalized);

            beepTimer -= Time.deltaTime;

            if (beepTimer <= 0f)
            {
                if (beepSource != null)
                    beepSource.Play();

                beepTimer = interval;
            }
        }
        else
        {
            beepTimer = 0f;
        }
    }

    void HandleScreenEffects()
    {
        if (vignetteImage != null)
        {
            float target = (currentTime <= 5f)
                ? 0.08f + Mathf.Sin(Time.time * 4f) * 0.04f
                : 0f;

            Color c = vignetteImage.color;
            c.a = Mathf.Lerp(c.a, target, Time.deltaTime * 3f);
            vignetteImage.color = c;
        }

        if (flashValue > 0f)
        {
            flashValue -= Time.deltaTime * 4f;

            if (flashImage != null)
            {
                Color c = flashImage.color;
                c.a = flashValue;
                flashImage.color = c;
            }
        }
    }

    void SpawnWave()
    {
        int spawnCount = baseSpawnCount + currentWave;

        flashValue = 0.2f;

        if (spawnWaveSource != null)
            spawnWaveSource.Play();

        for (int i = 0; i < spawnCount; i++)
        {
            Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];

            GameObject obj = Instantiate(macrophagePrefab, spawnPoint.position, Quaternion.identity);

            MacrophageFSM fsm = obj.GetComponent<MacrophageFSM>();

            if (Camera.main != null)
                fsm.player = Camera.main.transform;

            PlayerStats stats = FindObjectOfType<PlayerStats>();
            if (stats != null)
                fsm.playerStats = stats;

            if (paths.Length > 0)
            {
                Transform[] chosenPath = paths[UnityEngine.Random.Range(0, paths.Length)].points;
                fsm.patrolPoints = chosenPath;
            }
        }

        Debug.Log("Spawned Wave: " + currentWave);

        // ✅ Trigger UI event
        OnMacrophageWaveSpawn?.Invoke(currentWave);
    }
}