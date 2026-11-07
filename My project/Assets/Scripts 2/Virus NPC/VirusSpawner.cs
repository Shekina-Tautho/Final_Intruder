
using UnityEngine;

public class VirusSpawner : MonoBehaviour
{
    [Header("Virus Settings")]
    public GameObject virusPrefab;
    public int virusCount = 20;

    [Header("Original Spawn Area")]
    public BoxCollider spawnZone;

    [Header("Additional Spawn Points")]
    public Transform[] additionalSpawnPoints;

    [Tooltip("Random spawning distance around each additional spawn point.")]
    public float spawnRadius = 0.5f;

    private bool hasSpawned = false;

    public void StartSpawning()
    {
        if (hasSpawned)
        {
            Debug.Log("Viruses have already been spawned.");
            return;
        }

        if (virusPrefab == null)
        {
            Debug.LogError("Virus Prefab is not assigned!");
            return;
        }

        if (spawnZone == null &&
            (additionalSpawnPoints == null || additionalSpawnPoints.Length == 0))
        {
            Debug.LogError("No spawn zones or spawn points assigned!");
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
        Vector3 spawnPosition = GetRandomPosition();

        Instantiate(
            virusPrefab,
            spawnPosition,
            Quaternion.identity
        );
    }

    private Vector3 GetRandomPosition()
    {
        int availableLocations = additionalSpawnPoints != null
            ? additionalSpawnPoints.Length
            : 0;

        if (spawnZone != null)
        {
            availableLocations++;
        }

        int selectedLocation = Random.Range(0, availableLocations);

        // Use the original BoxCollider spawn area.
        if (spawnZone != null && selectedLocation == 0)
        {
            Bounds bounds = spawnZone.bounds;

            float x = Random.Range(bounds.min.x, bounds.max.x);
            float y = Random.Range(bounds.min.y, bounds.max.y);
            float z = Random.Range(bounds.min.z, bounds.max.z);

            return new Vector3(x, y, z);
        }

        // Use one of the additional spawn points.
        int pointIndex = selectedLocation - (spawnZone != null ? 1 : 0);

        Transform selectedPoint = additionalSpawnPoints[pointIndex];

        Vector3 randomOffset =
            Random.insideUnitSphere * spawnRadius;

        return selectedPoint.position + randomOffset;
    }
}
