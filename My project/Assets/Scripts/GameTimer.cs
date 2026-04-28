using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Header("Timer")]
    public float startTime = 60f;
    private float currentTime;

    private bool dangerMode = false;

    [Header("UI")]
    public TextMeshProUGUI timerText;

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
    }

    void Update()
    {
        currentTime -= Time.deltaTime;

        // Trigger wave
        if (currentTime <= 0f)
        {
            SpawnWave();
            currentWave++;
            currentTime = startTime;
        }

        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        int seconds = Mathf.CeilToInt(currentTime);
        timerText.text = seconds.ToString();

        // 🧠 DANGER MODE (last 5 seconds)
        if (currentTime <= 5f)
        {
            if (!dangerMode)
                dangerMode = true;

            timerText.color = Color.red;
        }
        else
        {
            if (dangerMode)
                dangerMode = false;

            timerText.color = Color.white;
        }
    }

    void SpawnWave()
    {
        int spawnCount = baseSpawnCount + currentWave;

        for (int i = 0; i < spawnCount; i++)
        {
            // Pick random spawn point
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Spawn macrophage
            GameObject obj = Instantiate(macrophagePrefab, spawnPoint.position, Quaternion.identity);

            MacrophageFSM fsm = obj.GetComponent<MacrophageFSM>();

            // Assign player (VR camera)
            if (Camera.main != null)
                fsm.player = Camera.main.transform;

            // Assign PlayerStats (from scene)
            PlayerStats stats = FindObjectOfType<PlayerStats>();
            if (stats != null)
                fsm.playerStats = stats;

            // Assign random patrol path
            if (paths.Length > 0)
            {
                Transform[] chosenPath = paths[Random.Range(0, paths.Length)].points;
                fsm.patrolPoints = chosenPath;
            }
        }

        Debug.Log("Spawned Wave: " + currentWave + " | Count: " + spawnCount);
    }
}