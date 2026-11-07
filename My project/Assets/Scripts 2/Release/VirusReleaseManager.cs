
using UnityEngine;

public class VirusReleaseManager : MonoBehaviour
{
    [Header("Player")]
    public Transform playerRoot;
    public CharacterController characterController;

    [Header("Epithelial Cells")]
    public Transform epithelialCellsRoot;

    [Header("Virus Spawner")]
    public VirusSpawner virusSpawner;

    [Header("Release Settings")]
    [Min(0)]
    public int releaseVirusCount = 20;

    public float playerReleaseHeight = 0.3f;

    private void Awake()
    {
        PrepareRelease();
    }

    private void PrepareRelease()
    {
        // Check whether Scene 1 saved a cell position.
        if (!VirusTransferData.hasAbsorbedCell)
        {
            Debug.LogError(
                "VirusReleaseManager: No absorbed cell position was saved. " +
                "Start from Scene 1 before entering Scene 3."
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

        // Convert the saved local position back into world space.
        Vector3 savedWorldPosition =
            epithelialCellsRoot.TransformPoint(
                VirusTransferData.absorbedCellLocalPosition
            );

        // Find the matching epithelial cell.
        SkinnedMeshRenderer[] cells =
            epithelialCellsRoot.GetComponentsInChildren<SkinnedMeshRenderer>();

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
                "VirusReleaseManager: Could not find the matching epithelial cell."
            );
            return;
        }

        // Get the matching cell center.
        Vector3 releasePosition = closestCell.bounds.center;

        // Position the player at the corresponding cell.
        if (characterController != null)
            characterController.enabled = false;

        playerRoot.position =
            releasePosition + Vector3.up * playerReleaseHeight;

        if (characterController != null)
            characterController.enabled = true;

        // Create a spawn point at the matching cell.
        GameObject spawnPointObject =
            new GameObject("VirusReleaseSpawnPoint");

        spawnPointObject.transform.position = releasePosition;

        // Configure the virus spawner for Scene 3.
        virusSpawner.spawnZone = null;

        virusSpawner.additionalSpawnPoints =
            new Transform[]
            {
                spawnPointObject.transform
            };

        // Override the original virus count for this scene.
        virusSpawner.virusCount = releaseVirusCount;

        Debug.Log(
            "Release location prepared at: " + releasePosition
        );

        Debug.Log(
            "Virus release count set to: " + releaseVirusCount
        );
    }
}
