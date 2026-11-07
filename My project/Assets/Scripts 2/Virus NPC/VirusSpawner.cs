
using UnityEngine;

public class VirusSpawner : MonoBehaviour
{
    [Header("Virus Settings")]
    public GameObject virusPrefab;
    public BoxCollider spawnZone;
    public int virusCount = 20;

    private bool hasSpawned = false;

    // Called when the player finishes the tutorial
    public void StartSpawning()
    {
        // Prevent viruses from spawning more than once
        if (hasSpawned)
        {
            Debug.Log("Viruses have already been spawned.");
            return;
        }

        // Check required references
        if (virusPrefab == null)
        {
            Debug.LogError("Virus Prefab is not assigned!");
            return;
        }

        if (spawnZone == null)
        {
            Debug.LogError("Spawn Zone is not assigned!");
            return;
        }

        hasSpawned = true;

        Debug.Log("Starting virus simulation...");

        for (int i = 0; i < virusCount; i++)
        {
            SpawnVirus();
        }

        Debug.Log(virusCount + " viruses spawned!");
    }

    private void SpawnVirus()
    {
        Vector3 spawnPosition = GetRandomPositionInZone();

        Instantiate(
            virusPrefab,
            spawnPosition,
            Quaternion.identity
        );
    }

    private Vector3 GetRandomPositionInZone()
    {
        Bounds bounds = spawnZone.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);
        float z = Random.Range(bounds.min.z, bounds.max.z);

        return new Vector3(x, y, z);
    }
}
