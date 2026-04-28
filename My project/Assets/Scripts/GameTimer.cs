using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    [Header("Timer")]
    public float startTime = 60f;
    private float currentTime;

    private bool dangerMode = false;

    [Header("UI")]
    public TextMeshProUGUI timerText;

    [Header("VR Screen Effects (attach from World Space Canvas)")]
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

    void Start()
    {
        currentTime = startTime;
        beepTimer = 0f;
    }

    void Update()
    {
        currentTime -= Time.deltaTime;

        HandleDangerMode();
        HandleBeepWarning();
        HandleScreenEffects(); // 🔥 VR effects

        if (currentTime <= 0f)
        {
            SpawnWave();
            currentWave++;
            currentTime = startTime;
            beepTimer = 0f;
        }

        UpdateTimerUI();
    }

    // ---------------- TIMER UI ----------------
    void UpdateTimerUI()
    {
        int seconds = Mathf.CeilToInt(currentTime);
        timerText.text = seconds.ToString();
    }

    // ---------------- DANGER MODE ----------------
    void HandleDangerMode()
    {
        if (currentTime <= 5f)
        {
            dangerMode = true;
            timerText.color = Color.red;
        }
        else
        {
            dangerMode = false;
            timerText.color = Color.white;
        }
    }

    // ---------------- BEEP SYSTEM ----------------
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

    // ---------------- VR SCREEN EFFECTS ----------------
    void HandleScreenEffects()
    {
        // 🔴 DANGER VIGNETTE (breathing effect)
        if (vignetteImage != null)
        {
            if (currentTime <= 5f)
            {
                float pulse = 0.08f + Mathf.Sin(Time.time * 4f) * 0.04f;

                Color c = vignetteImage.color;
                c.a = Mathf.Lerp(c.a, pulse, Time.deltaTime * 3f);
                vignetteImage.color = c;
            }
            else
            {
                Color c = vignetteImage.color;
                c.a = Mathf.Lerp(c.a, 0f, Time.deltaTime * 5f);
                vignetteImage.color = c;
            }
        }

        // ⚡ SPAWN FLASH
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

    // ---------------- SPAWNING ----------------
    void SpawnWave()
    {
        int spawnCount = baseSpawnCount + currentWave;

        // ⚡ VR IMMUNE RESPONSE FLASH
        flashValue = 0.2f;

        // 🔊 wave spawn sound
        if (spawnWaveSource != null)
            spawnWaveSource.Play();

        for (int i = 0; i < spawnCount; i++)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            GameObject obj = Instantiate(macrophagePrefab, spawnPoint.position, Quaternion.identity);

            MacrophageFSM fsm = obj.GetComponent<MacrophageFSM>();

            if (Camera.main != null)
                fsm.player = Camera.main.transform;

            PlayerStats stats = FindObjectOfType<PlayerStats>();
            if (stats != null)
                fsm.playerStats = stats;

            if (paths.Length > 0)
            {
                Transform[] chosenPath = paths[Random.Range(0, paths.Length)].points;
                fsm.patrolPoints = chosenPath;
            }
        }

        Debug.Log("Spawned Wave: " + currentWave + " | Count: " + spawnCount);
    }
}