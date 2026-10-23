using UnityEngine;
using System.Collections;

public class PlayerCellSnapper : MonoBehaviour
{
    [Header("Player")]
    public Transform playerRoot;
    public CharacterController characterController;

    [Header("Epithelial Cells")]
    public Transform epithelialCellsRoot;

    [Header("Controls to Disable")]
    public MonoBehaviour[] controlsToDisable;

    [Header("Movement Settings")]
    public float animationWaitTime = 1f;
    public float movementDuration = 2f;

    private bool isMovingToCell;

    public void MoveToCell(Receptor triggeredReceptor)
    {
        if (isMovingToCell)
            return;

        if (triggeredReceptor == null ||
            playerRoot == null ||
            epithelialCellsRoot == null)
        {
            Debug.LogWarning(
                "PlayerCellSnapper: Assign all required references."
            );
            return;
        }

        SkinnedMeshRenderer[] cells =
            epithelialCellsRoot.GetComponentsInChildren<SkinnedMeshRenderer>();

        SkinnedMeshRenderer closestCell = null;
        float closestDistance = Mathf.Infinity;

        // Find the epithelial cell closest to the triggered receptor.
        foreach (SkinnedMeshRenderer cell in cells)
        {
            if (cell == null)
                continue;

            float distance = Vector3.Distance(
                triggeredReceptor.transform.position,
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
            Debug.LogWarning(
                "PlayerCellSnapper: No epithelial cells found."
            );
            return;
        }

        Vector3 targetCenter = closestCell.bounds.center;

        StartCoroutine(MoveAfterAnimation(targetCenter));
    }

    private IEnumerator MoveAfterAnimation(Vector3 targetCenter)
    {
        isMovingToCell = true;

        // Let the receptor animation play before moving.
        yield return new WaitForSeconds(animationWaitTime);

        // Disable movement controls only during the movement.
        foreach (MonoBehaviour control in controlsToDisable)
        {
            if (control != null)
                control.enabled = false;
        }

        if (characterController != null)
            characterController.enabled = false;

        Vector3 startPosition = playerRoot.position;
        Vector3 targetPosition =
            startPosition + (targetCenter - startPosition);

        float elapsedTime = 0f;

        // Move smoothly instead of snapping instantly.
        while (elapsedTime < movementDuration)
        {
            elapsedTime += Time.deltaTime;

            float progress = Mathf.Clamp01(
                elapsedTime / movementDuration
            );

            // Smooth start and stop.
            float smoothProgress =
                progress * progress * (3f - 2f * progress);

            playerRoot.position = Vector3.Lerp(
                startPosition,
                targetPosition,
                smoothProgress
            );

            yield return null;
        }

        playerRoot.position = targetPosition;

        if (characterController != null)
            characterController.enabled = true;

        // Restore movement controls after arriving.
        foreach (MonoBehaviour control in controlsToDisable)
        {
            if (control != null)
                control.enabled = true;
        }

        isMovingToCell = false;

        Debug.Log("Player has reached the epithelial cell center.");
    }
}