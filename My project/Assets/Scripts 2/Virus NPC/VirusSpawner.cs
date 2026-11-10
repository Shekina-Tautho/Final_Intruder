
using System.Collections.Generic;
using UnityEngine;

public class VirusSpawner : MonoBehaviour
{
    [Header("Virus Settings")]
    public GameObject virusPrefab;
    public int virusCount = 20;

    [Header("Released Virus Scale")]
    public Vector3 releasedVirusScale =
        new Vector3(0.25f, 0.25f, 0.25f);

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

    [Header("Release Spawn Distribution")]
    [Tooltip("Maximum distance of release viruses from the player.")]
    [Min(0f)]
    public float releaseSpawnRadius = 0.8f;

    [Tooltip("Controls the amount of vertical variation in initial spawn positions.")]
    [Range(0f, 1f)]
    public float releaseSpawnHeightVariation = 0.4f;

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
        Vector3 spawnPosition = GetRandomPosition(index);

        GameObject spawnedVirus = Instantiate(
            virusPrefab,
            spawnPosition,
            Quaternion.identity
        );

        // ONLY released viruses receive a scale adjustment.
        // Outside NPCs retain their original prefab scale.
        if (releaseMode)
        {
            spawnedVirus.transform.localScale =
                releasedVirusScale;
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

        // ORIGINAL OUTSIDE NPC BEHAVIOR.
        // No scale adjustment is applied.
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

    private Vector3 GetRandomPosition(int index)
    {
        // RELEASE VIRUSES:
        // Distribute them around the player in a loose formation.
        if (releaseMode && releaseSpawnCenter != null)
        {
            float goldenAngle = 137.5f;

            // Give each virus a different horizontal direction.
            float angle = index * goldenAngle * Mathf.Deg2Rad;

            // Add slight variation in height.
            float vertical = Random.Range(
                -releaseSpawnHeightVariation,
                releaseSpawnHeightVariation
            );

            Vector3 offsetDirection = new Vector3(
                Mathf.Cos(angle),
                vertical,
                Mathf.Sin(angle)
            ).normalized;

            // Randomize distance without placing viruses too close
            // to the exact center.
            float minimumRadius = releaseSpawnRadius * 0.65f;

            float randomRadius = Random.Range(
                minimumRadius,
                releaseSpawnRadius
            );

            Vector3 randomOffset =
                offsetDirection * randomRadius;

            return releaseSpawnCenter.position + randomOffset;
        }

        // ORIGINAL OUTSIDE NPC SPAWNING.
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
