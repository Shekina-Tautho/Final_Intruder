
using System.Collections.Generic;
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
    public float spawnRadius = 0.5f;

    [Header("Release Settings")]
    public bool releaseMode = false;
    public Transform releaseSpawnCenter;
    public Vector3 releaseOrigin;
    public Vector3 releaseDirection;
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
            Debug.LogError("Virus Prefab is not assigned!");
            return;
        }

        if (releaseMode && releaseSpawnCenter == null)
        {
            Debug.LogError("Release Spawn Center is not assigned!");
            return;
        }

        int pointCount = additionalSpawnPoints != null
            ? additionalSpawnPoints.Length
            : 0;

        if (!releaseMode && spawnZone == null && pointCount == 0)
        {
            Debug.LogError("No spawn zones or spawn points assigned!");
            return;
        }

        hasSpawned = true;

        spawnedVirusTransforms.Clear();
        spawnedStartPositions.Clear();

        Debug.Log(
            "Starting virus simulation. Release mode: " + releaseMode
        );

        for (int i = 0; i < virusCount; i++)
        {
            SpawnVirus(i);
        }

        Debug.Log(virusCount + " viruses spawned!");
    }

    private void SpawnVirus(int index)
    {
        Vector3 spawnPosition = GetRandomPosition();

        GameObject spawnedVirus = Instantiate(
            virusPrefab,
            spawnPosition,
            Quaternion.identity
        );

        spawnedVirusTransforms.Add(spawnedVirus.transform);
        spawnedStartPositions.Add(spawnedVirus.transform.position);

        if (releaseMode)
        {
            Debug.Log(
                "[RELEASE NPC " + (index + 1) +
                "] START POSITION: " +
                spawnedVirus.transform.position
            );
        }

        // Keep the original behavior for outside NPCs.
        if (!releaseMode)
            return;

        // Disable scripts that could attach or move
        // release NPCs toward epithelial cells.
        if (disableAutoAttachOnSpawn)
        {
            NPCReceptorAutoAttach autoAttach =
                spawnedVirus.GetComponent<NPCReceptorAutoAttach>();

            if (autoAttach != null)
                autoAttach.enabled = false;

            NPCCellAbsorber cellAbsorber =
                spawnedVirus.GetComponent<NPCCellAbsorber>();

            if (cellAbsorber != null)
                cellAbsorber.enabled = false;

            Debug.Log(
                "[RELEASE NPC " + (index + 1) +
                "] Auto-attach and cell absorption disabled."
            );
        }

        // Start release movement.
        VirusNPC virusMovement =
            spawnedVirus.GetComponent<VirusNPC>();

        if (virusMovement != null)
        {
            virusMovement.BeginRelease(releaseDirection);
        }
        else
        {
            Debug.LogWarning(
                "Spawned NPC has no VirusNPC component: " +
                spawnedVirus.name
            );
        }
    }

    private Vector3 GetRandomPosition()
    {
        // Release NPCs spawn around the player.
        if (releaseMode && releaseSpawnCenter != null)
        {
            Vector3 randomOffset =
                Random.insideUnitSphere * spawnRadius;

            return releaseSpawnCenter.position + randomOffset;
        }

        // Original outside NPC spawning logic.
        int pointCount = additionalSpawnPoints != null
            ? additionalSpawnPoints.Length
            : 0;

        int availableLocations = pointCount;

        if (spawnZone != null)
            availableLocations++;

        if (availableLocations == 0)
            return Vector3.zero;

        int selectedLocation = Random.Range(
            0,
            availableLocations
        );

        if (spawnZone != null && selectedLocation == 0)
        {
            Bounds bounds = spawnZone.bounds;

            float x = Random.Range(bounds.min.x, bounds.max.x);
            float y = Random.Range(bounds.min.y, bounds.max.y);
            float z = Random.Range(bounds.min.z, bounds.max.z);

            return new Vector3(x, y, z);
        }

        int pointIndex = selectedLocation -
            (spawnZone != null ? 1 : 0);

        Transform selectedPoint =
            additionalSpawnPoints[pointIndex];

        if (selectedPoint == null)
            return Vector3.zero;

        Vector3 offset =
            Random.insideUnitSphere * spawnRadius;

        return selectedPoint.position + offset;
    }
}
