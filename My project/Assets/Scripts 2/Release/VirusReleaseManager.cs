
using System.Collections;
using UnityEngine;

public class VirusReleaseManager : MonoBehaviour
{
    [Header("Player")]
    public Transform playerRoot;
    public CharacterController characterController;

    [Tooltip("Add only the scripts responsible for player movement.")]
    public MonoBehaviour[] controlsToDisable;

    [Header("Epithelial Cells")]
    public Transform epithelialCellsRoot;

    [Header("Virus Spawner")]
    public VirusSpawner virusSpawner;

    [Header("Release Settings")]
    [Min(0)]
    public int releaseVirusCount = 20;

    [Min(0.1f)]
    public float playerReleaseDistance = 1.5f;

    [Min(0.1f)]
    public float playerReleaseDuration = 1.5f;

    private Vector3 releasePosition;
    private Vector3 epithelialCenter;
    private Vector3 releaseDirection;

    private bool isPrepared;
    private bool releaseStarted;

    private void Awake()
    {
        SetPlayerControls(false);
        PrepareRelease();
    }

    private void PrepareRelease()
    {
        if (!VirusTransferData.hasAbsorbedCell)
        {
            Debug.LogError(
                "VirusReleaseManager: No absorbed cell position was saved."
            );
            return;
        }

        if (epithelialCellsRoot == null ||
            playerRoot == null ||
            virusSpawner == null)
        {
            Debug.LogError(
                "VirusReleaseManager: Assign all required references."
            );
            return;
        }

        Vector3 savedWorldPosition =
            epithelialCellsRoot.TransformPoint(
                VirusTransferData.absorbedCellLocalPosition
            );

        SkinnedMeshRenderer[] cells =
            epithelialCellsRoot.GetComponentsInChildren<SkinnedMeshRenderer>();

        if (cells.Length == 0)
        {
            Debug.LogError(
                "VirusReleaseManager: No epithelial cell renderers found."
            );
            return;
        }

        SkinnedMeshRenderer closestCell = null;
        float closestDistance = Mathf.Infinity;

        // Find the matching cell.
        foreach (SkinnedMeshRenderer cell in cells)
        {
            if (cell == null)
                continue;

            float distance = Vector3.Distance(
                savedWorldPosition,
                cell.bounds.center
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestCell = cell;
            }
        }

        if (closestCell == null)
        {
            Debug.LogError(
                "VirusReleaseManager: Could not find matching cell."
            );
            return;
        }

        releasePosition = closestCell.bounds.center;

        // Calculate the center of the entire epithelial arrangement.
        Vector3 totalCenter = Vector3.zero;
        int validCellCount = 0;

        foreach (SkinnedMeshRenderer cell in cells)
        {
            if (cell == null)
                continue;

            totalCenter += cell.bounds.center;
            validCellCount++;
        }

        if (validCellCount == 0)
        {
            Debug.LogError(
                "VirusReleaseManager: Could not calculate epithelial center."
            );
            return;
        }

        epithelialCenter = totalCenter / validCellCount;

        // Direction from absorbed cell toward the arrangement center.
        releaseDirection =
            epithelialCenter - releasePosition;

        if (releaseDirection.sqrMagnitude < 0.001f)
        {
            Debug.LogError(
                "VirusReleaseManager: Absorbed cell is at the calculated center. " +
                "A release direction cannot be calculated."
            );
            return;
        }

        releaseDirection.Normalize();

        // Keep the player inside the cell before release.
        if (characterController != null)
            characterController.enabled = false;

        playerRoot.position = releasePosition;

        // Configure NPC spawn point.
        GameObject spawnPointObject =
            new GameObject("VirusReleaseSpawnPoint");

        spawnPointObject.transform.position = releasePosition;

        virusSpawner.spawnZone = null;

        virusSpawner.additionalSpawnPoints =
            new Transform[]
            {
                spawnPointObject.transform
            };

        virusSpawner.virusCount = releaseVirusCount;
        virusSpawner.releaseMode = true;
        virusSpawner.releaseOrigin = releasePosition;

        // All NPCs will move in the same direction as the player.
        virusSpawner.releaseDirection = releaseDirection;

        virusSpawner.disableAutoAttachOnSpawn = true;

        isPrepared = true;

        Debug.Log("Release cell: " + closestCell.name);
        Debug.Log("Release position: " + releasePosition);
        Debug.Log("Epithelial center: " + epithelialCenter);
        Debug.Log("Release direction: " + releaseDirection);
    }

    public void BeginRelease()
    {
        if (!isPrepared)
        {
            Debug.LogError(
                "VirusReleaseManager: Release was not prepared."
            );
            return;
        }

        if (releaseStarted)
        {
            Debug.LogWarning(
                "VirusReleaseManager: Release has already started."
            );
            return;
        }

        releaseStarted = true;

        SetPlayerControls(false);

        // Spawn NPC viruses.
        virusSpawner.StartSpawning();

        // Start player release.
        StartCoroutine(ReleasePlayer());
    }

    private IEnumerator ReleasePlayer()
    {
        Vector3 startPosition = releasePosition;

        Vector3 endPosition =
            startPosition +
            releaseDirection * playerReleaseDistance;

        float elapsed = 0f;

        while (elapsed < playerReleaseDuration)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(
                elapsed / playerReleaseDuration
            );

            float smoothProgress =
                Mathf.SmoothStep(0f, 1f, progress);

            playerRoot.position = Vector3.Lerp(
                startPosition,
                endPosition,
                smoothProgress
            );

            yield return null;
        }

        playerRoot.position = endPosition;

        // Restore CharacterController.
        if (characterController != null)
            characterController.enabled = true;

        // Restore player movement.
        SetPlayerControls(true);

        Debug.Log(
            "Player release complete. Movement enabled."
        );
    }

    private void SetPlayerControls(bool enabledState)
    {
        if (controlsToDisable == null)
            return;

        foreach (MonoBehaviour control in controlsToDisable)
        {
            if (control != null)
                control.enabled = enabledState;
        }
    }
}
