using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Header("Timer")]
    public float startTime = 60f;
    private float currentTime;

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

        if (currentTime <= 0f)
        {
            SpawnWave();
            currentWave++;
            currentTime = startTime; // reset timer (looping waves)
        }

        UpdateTimerUI();
    }

    void UpdateTimerUI()
    {
        int seconds = Mathf.CeilToInt(currentTime);
        timerText.text = seconds.ToString();
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

            // Assign PlayerStats (from XR Origin)
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