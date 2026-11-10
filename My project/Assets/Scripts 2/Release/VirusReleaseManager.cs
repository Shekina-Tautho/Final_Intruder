
using System.Collections;
using UnityEngine;

public class VirusReleaseManager : MonoBehaviour
{
    [Header("Player")]
    public Transform playerRoot;
    public Transform playerHead;
    public CharacterController characterController;

    [Tooltip("Add only the scripts responsible for player movement.")]
    public MonoBehaviour[] controlsToDisable;

    [Header("Epithelial Cells")]
    public Transform epithelialCellsRoot;

    [Header("Outside NPC Spawner")]
    public VirusSpawner outsideVirusSpawner;

    [Header("Release NPC Spawner")]
    public VirusSpawner virusSpawner;

    [Header("Scene Fade")]
    public VirusReleaseSceneFade sceneFade;

    [Header("Simulation UI")]
    public SimulationUIController simulationUI;

    [Header("Release Settings")]
    [Min(0)]
    public int releaseVirusCount = 20;

    [Min(0.1f)]
    public float playerReleaseDistance = 1.5f;

    [Min(0.1f)]
    public float playerReleaseDuration = 4f;

    [Header("Release Speed Profile")]
    public float fastPhaseDuration = 2f;
    public float fastSpeedMultiplier = 1.6f;
    public float slowSpeedMultiplier = 0.55f;

    private Vector3 releasePosition;
    private Vector3 epithelialCenter;
    private Vector3 releaseDirection;

    private bool isPrepared;
    private bool releaseStarted;

    private void Awake()
    {
        SetPlayerControls(false);

        if (playerHead == null && Camera.main != null)
        {
            playerHead = Camera.main.transform;
        }

        // Spawn the original outside NPC viruses.
        if (outsideVirusSpawner != null)
        {
            outsideVirusSpawner.StartSpawning();
            Debug.Log("Outside NPC viruses spawned.");
        }
        else
        {
            Debug.LogError(
                "VirusReleaseManager: Outside Virus Spawner is not assigned!"
            );
        }

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

        // Find the epithelial cell closest to the saved absorbed-cell position.
        SkinnedMeshRenderer closestCell = null;
        float closestDistance = Mathf.Infinity;

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

        // Calculate the average center of all epithelial cells.
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

        // Direction points inward, toward the epithelial center.
        releaseDirection = epithelialCenter - releasePosition;

        if (releaseDirection.sqrMagnitude < 0.001f)
        {
            Debug.LogError(
                "VirusReleaseManager: Could not calculate release direction."
            );
            return;
        }

        releaseDirection.Normalize();

        // Position the player at the matching epithelial cell.
        if (characterController != null)
            characterController.enabled = false;

        playerRoot.position = releasePosition;

        // Configure the separate release-virus spawner.
        virusSpawner.releaseSpawnCenter = playerRoot;
        virusSpawner.spawnZone = null;
        virusSpawner.additionalSpawnPoints = null;

        virusSpawner.virusCount = releaseVirusCount;
        virusSpawner.releaseMode = true;
        virusSpawner.releaseOrigin = playerRoot.position;
        virusSpawner.releaseDirection = releaseDirection;

        // Keep player and release viruses synchronized.
        virusSpawner.releaseMovementDistance = playerReleaseDistance;
        virusSpawner.releaseMovementDuration = playerReleaseDuration;

        isPrepared = true;

        Debug.Log("========== RELEASE PREPARATION ==========");
        Debug.Log("Matching epithelial cell: " + closestCell.name);
        Debug.Log("Saved cell position: " + savedWorldPosition);
        Debug.Log("Player starting position: " + playerRoot.position);

        if (playerHead != null)
        {
            Debug.Log("Player HEAD starting position: " + playerHead.position);
        }

        Debug.Log("Epithelial center: " + epithelialCenter);
        Debug.Log("Release direction: " + releaseDirection);
        Debug.Log("Release distance: " + playerReleaseDistance);
        Debug.Log("Release duration: " + playerReleaseDuration);
        Debug.Log("==========================================");
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

        // Configure the shared speed profile BEFORE spawning the NPCs.
        ReleaseMovementProfile.Configure(
            fastPhaseDuration,
            fastSpeedMultiplier,
            slowSpeedMultiplier
        );

        Debug.Log("========== RELEASE START ==========");
        Debug.Log("PLAYER START: " + playerRoot.position);

        if (playerHead != null)
        {
            Debug.Log("PLAYER HEAD START: " + playerHead.position);
        }

        // Spawn the new ReleaseVirus prefabs.
        virusSpawner.StartSpawning();

        Debug.Log(
            "Total tracked release NPCs: " +
            virusSpawner.spawnedVirusTransforms.Count
        );

        // Start player movement.
        StartCoroutine(ReleasePlayer());

        // Start the fade-in at the same time as movement.
        if (sceneFade != null)
        {
            sceneFade.BeginFadeIn();
        }
        else
        {
            Debug.LogWarning(
                "VirusReleaseManager: Scene Fade is not assigned."
            );
        }
    }

    private IEnumerator ReleasePlayer()
    {
        Vector3 startPosition = releasePosition;

        Vector3 endPosition =
            startPosition +
            releaseDirection * playerReleaseDistance;

        Debug.Log("PLAYER TARGET END POSITION: " + endPosition);

        float elapsed = 0f;

        while (elapsed < playerReleaseDuration)
        {
            elapsed += Time.deltaTime;

            float progress = ReleaseMovementProfile.GetProgress(
                elapsed,
                playerReleaseDuration
            );

            playerRoot.position = Vector3.Lerp(
                startPosition,
                endPosition,
                progress
            );

            yield return null;
        }

        playerRoot.position = endPosition;

        // Update simulation stage from 6 (Release) to 7 (Spread).
        if (simulationUI != null)
        {
            simulationUI.SetStage(7);
        }

        if (characterController != null)
            characterController.enabled = true;

        SetPlayerControls(true);

        Debug.Log("========== RELEASE POSITION COMPARISON ==========");
        Debug.Log("PLAYER FINAL POSITION: " + playerRoot.position);

        if (playerHead != null)
        {
            Debug.Log("PLAYER HEAD FINAL POSITION: " + playerHead.position);
        }

        for (int i = 0;
             i < virusSpawner.spawnedVirusTransforms.Count;
             i++)
        {
            Transform npc = virusSpawner.spawnedVirusTransforms[i];

            if (npc == null)
            {
                Debug.LogWarning(
                    "[RELEASE NPC " + (i + 1) + "] No longer exists."
                );
                continue;
            }

            Vector3 npcStart =
                virusSpawner.spawnedStartPositions[i];

            Vector3 npcFinal = npc.position;

            float distanceFromPlayerRoot = Vector3.Distance(
                npcFinal,
                playerRoot.position
            );

            Debug.Log(
                "[RELEASE NPC " + (i + 1) + "]" +
                "\nSTART: " + npcStart +
                "\nFINAL: " + npcFinal +
                "\nDISTANCE FROM PLAYER ROOT: " +
                distanceFromPlayerRoot + " units"
            );

            if (playerHead != null)
            {
                float distanceFromHead = Vector3.Distance(
                    npcFinal,
                    playerHead.position
                );

                Debug.Log(
                    "[RELEASE NPC " + (i + 1) +
                    "] DISTANCE FROM PLAYER HEAD: " +
                    distanceFromHead + " units"
                );
            }
        }

        Debug.Log("==================================================");
        Debug.Log("Player release complete. Movement enabled.");
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
