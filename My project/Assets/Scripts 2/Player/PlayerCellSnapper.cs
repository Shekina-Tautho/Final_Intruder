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

    [Header("Endocytosis Animation")]
    [Range(0f, 100f)]
    public float endocytosisTargetWeight = 100f;

    public float endocytosisDuration = 2f;

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

        StartCoroutine(
            MoveAfterAnimation(closestCell)
        );
    }

    private IEnumerator MoveAfterAnimation(
        SkinnedMeshRenderer targetCell)
    {
        isMovingToCell = true;

        // Allow the receptor animation to play first.
        yield return new WaitForSeconds(animationWaitTime);

        foreach (MonoBehaviour control in controlsToDisable)
        {
            if (control != null)
                control.enabled = false;
        }

        if (characterController != null)
            characterController.enabled = false;

        // Start the epithelial endocytosis animation.
        StartCoroutine(AnimateEndocytosis(targetCell));

        Vector3 targetCenter = targetCell.bounds.center;
        Vector3 startPosition = playerRoot.position;

        Vector3 targetPosition =
            startPosition + (targetCenter - startPosition);

        float elapsedTime = 0f;

        // Smoothly move the player toward the cell center.
        while (elapsedTime < movementDuration)
        {
            elapsedTime += Time.deltaTime;

            float progress = Mathf.Clamp01(
                elapsedTime / movementDuration
            );

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

        foreach (MonoBehaviour control in controlsToDisable)
        {
            if (control != null)
                control.enabled = true;
        }

        isMovingToCell = false;

        Debug.Log("Player reached the epithelial cell center.");
    }

    private IEnumerator AnimateEndocytosis(
        SkinnedMeshRenderer targetCell)
    {
        Mesh mesh = targetCell.sharedMesh;

        if (mesh == null)
            yield break;

        int blendShapeIndex =
            mesh.GetBlendShapeIndex("Endocytosis");

        if (blendShapeIndex < 0)
        {
            Debug.LogWarning(
                targetCell.gameObject.name +
                " does not have an Endocytosis blend shape."
            );
            yield break;
        }

        float startWeight =
            targetCell.GetBlendShapeWeight(blendShapeIndex);

        float elapsedTime = 0f;

        while (elapsedTime < endocytosisDuration)
        {
            elapsedTime += Time.deltaTime;

            float progress = Mathf.Clamp01(
                elapsedTime / endocytosisDuration
            );

            float weight = Mathf.Lerp(
                startWeight,
                endocytosisTargetWeight,
                progress
            );

            targetCell.SetBlendShapeWeight(
                blendShapeIndex,
                weight
            );

            yield return null;
        }

        targetCell.SetBlendShapeWeight(
            blendShapeIndex,
            endocytosisTargetWeight
        );
    }
}