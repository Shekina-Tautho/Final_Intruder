
using System.Collections.Generic;
using UnityEngine;

public class VirusSpawner : MonoBehaviour
{
    [Header("Virus Settings")]
    public GameObject virusPrefab;
    public int virusCount = 20;

    [Header("Spawned Virus Scale")]
    public bool overrideSpawnScale = true;
    public Vector3 spawnedVirusScale = new Vector3(0.25f, 0.25f, 0.25f);

    [Header("Original Spawn Area")]
    public BoxCollider spawnZone;

    [Header("Additional Spawn Points")]
    public Transform[] additionalSpawnPoints;
    public float spawnRadius = 0.5f;

    [Header("Release Settings")]
    public bool releaseMode = false;
    public Transform releaseSpawnCenter;
    public Vector3 releaseOrigin;
    public Vector3 releaseDirection;

    [Header("Synchronized Release Movement")]
    public float releaseMovementDistance = 1.5f;
    public float releaseMovementDuration = 1.5f;

    [Header("Legacy NPC Settings")]
    public bool disableAutoAttachOnSpawn = false;

    [Header("Runtime Position Tracking")]
    public List<Transform> spawnedVirusTransforms =
        new List<Transform>();

    public List<Vector3> spawnedStartPositions =
        new List<Vector3>();

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
            Debug.LogError(
                "VirusSpawner: Virus Prefab is not assigned!"
            );
            return;
        }

        if (releaseMode && releaseSpawnCenter == null)
        {
            Debug.LogError(
                "VirusSpawner: Release Spawn Center is not assigned!"
            );
            return;
        }

        int pointCount = additionalSpawnPoints != null
            ? additionalSpawnPoints.Length
            : 0;

        if (!releaseMode && spawnZone == null && pointCount == 0)
        {
            Debug.LogError(
                "VirusSpawner: No spawn zones or spawn points assigned!"
            );
            return;
        }

        spawnedVirusTransforms.Clear();
        spawnedStartPositions.Clear();

        Debug.Log(
            "Starting virus simulation. Release mode: " + releaseMode
        );

        for (int i = 0; i < virusCount; i++)
        {
            SpawnVirus(i);
        }

        hasSpawned = true;

        Debug.Log(
            virusCount + " viruses spawned successfully!"
        );
    }

    private void SpawnVirus(int index)
    {
        Vector3 spawnPosition = GetRandomPosition();

        GameObject spawnedVirus = Instantiate(
            virusPrefab,
            spawnPosition,
            Quaternion.identity
        );

        // Apply scale only to this spawned instance.
        if (overrideSpawnScale)
        {
            spawnedVirus.transform.localScale = spawnedVirusScale;
        }

        spawnedVirusTransforms.Add(spawnedVirus.transform);

        spawnedStartPositions.Add(
            spawnedVirus.transform.position
        );

        if (releaseMode)
        {
            Debug.Log(
                "[RELEASE VIRUS " + (index + 1) + "]" +
                "\nSTART POSITION: " +
                spawnedVirus.transform.position +
                "\nDISTANCE FROM PLAYER: " +
                Vector3.Distance(
                    spawnedVirus.transform.position,
                    releaseSpawnCenter.position
                ) +
                "\nSPAWNED SCALE: " +
                spawnedVirus.transform.localScale
            );

            StartReleaseVirus(spawnedVirus, index);

            return;
        }

        // ORIGINAL OUTSIDE NPC BEHAVIOR
        // Keep the existing infection simulation unchanged.

        Debug.Log(
            "[OUTSIDE NPC " + (index + 1) +
            "] Spawned at: " +
            spawnedVirus.transform.position
        );
    }

    private void StartReleaseVirus(
        GameObject spawnedVirus,
        int index)
    {
        // The release prefab must have its own movement script.

        ReleaseVirusMovement movement =
            spawnedVirus.GetComponent<ReleaseVirusMovement>();

        if (movement == null)
        {
            Debug.LogError(
                "[RELEASE VIRUS " + (index + 1) + "] " +
                "ReleaseVirusMovement is missing from prefab: " +
                spawnedVirus.name
            );

            return;
        }

        // Give each virus its own spreading position.
        // All viruses retain synchronized forward movement.

        movement.BeginRelease(
            releaseDirection,
            releaseMovementDistance,
            releaseMovementDuration,
            index,
            virusCount
        );

        Debug.Log(
            "[RELEASE VIRUS " + (index + 1) + "] " +
            "Synchronized release and progressive spreading started."
        );
    }

    private Vector3 GetRandomPosition()
    {
        // NEW RELEASE VIRUSES:
        // Spawn around the player.

        if (releaseMode && releaseSpawnCenter != null)
        {
            Vector3 randomOffset =
                Random.insideUnitSphere * spawnRadius;

            return releaseSpawnCenter.position + randomOffset;
        }

        // ORIGINAL OUTSIDE NPC SPAWNING:
        // Preserve the existing spawn zone and spawn point logic.

        int pointCount = additionalSpawnPoints != null
            ? additionalSpawnPoints.Length
            : 0;

        int availableLocations = pointCount;

        if (spawnZone != null)
        {
            availableLocations++;
        }

        if (availableLocations == 0)
        {
            return Vector3.zero;
        }

        int selectedLocation = Random.Range(
            0,
            availableLocations
        );

        if (spawnZone != null && selectedLocation == 0)
        {
            Bounds bounds = spawnZone.bounds;

            float x = Random.Range(
                bounds.min.x,
                bounds.max.x
            );

            float y = Random.Range(
                bounds.min.y,
                bounds.max.y
            );

            float z = Random.Range(
                bounds.min.z,
                bounds.max.z
            );

            return new Vector3(x, y, z);
        }

        int pointIndex = selectedLocation -
            (spawnZone != null ? 1 : 0);

        Transform selectedPoint =
            additionalSpawnPoints[pointIndex];

        if (selectedPoint == null)
        {
            return Vector3.zero;
        }

        Vector3 offset =
            Random.insideUnitSphere * spawnRadius;

        return selectedPoint.position + offset;
    }
}
