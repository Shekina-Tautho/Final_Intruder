
using UnityEngine;
using System.Collections;

public class NPCCellAbsorber : MonoBehaviour
{
    [Header("Epithelial Cells")]
    public Transform epithelialCellsRoot;

    void Start()
    {
        if (epithelialCellsRoot == null)
        {
            GameObject epithelialObject = GameObject.Find("Epithelial");

            if (epithelialObject != null)
            {
                epithelialCellsRoot = epithelialObject.transform;
            }
            else
            {
                Debug.LogError(
                    "NPCCellAbsorber: Could not find an object named 'epithelial'."
                );
            }
        }
    }

    [Header("Absorption Timing")]
    public float animationWaitTime = 1f;
    public float movementDuration = 2f;

    [Header("Endocytosis Animation")]
    [Range(0f, 100f)]
    public float endocytosisTargetWeight = 100f;
    public float endocytosisDuration = 2f;
    public float endocytosisResetDuration = 2f;

    public bool IsAbsorbing { get; private set; }

    public void AbsorbIntoCell(Receptor triggeredReceptor)
    {
        if (IsAbsorbing || triggeredReceptor == null)
            return;

        if (epithelialCellsRoot == null)
        {
            Debug.LogWarning(
                "NPCCellAbsorber: Assign the epithelial cells root."
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
                "NPCCellAbsorber: No epithelial cells found."
            );
            return;
        }

        IsAbsorbing = true;
        StartCoroutine(AbsorptionSequence(closestCell));
    }

    private IEnumerator AbsorptionSequence(
        SkinnedMeshRenderer targetCell)
    {
        // Match the player's wait after receptor activation.
        yield return new WaitForSeconds(animationWaitTime);

        StartCoroutine(AnimateEndocytosis(targetCell));

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = targetCell.bounds.center;
        float elapsedTime = 0f;

        while (elapsedTime < movementDuration)
        {
            if (targetCell == null)
            {
                Destroy(gameObject);
                yield break;
            }

            elapsedTime += Time.deltaTime;

            float progress = Mathf.Clamp01(
                elapsedTime / movementDuration
            );

            float smoothProgress =
                progress * progress * (3f - 2f * progress);

            transform.position = Vector3.Lerp(
                startPosition,
                targetPosition,
                smoothProgress
            );

            yield return null;
        }

        if (targetCell != null)
            transform.position = targetCell.bounds.center;

        // Allow the endocytosis animation to finish.
        yield return new WaitForSeconds(endocytosisDuration);

        // Smoothly reset the Endocytosis blend shape to 0.
        if (targetCell != null)
        {
            Mesh mesh = targetCell.sharedMesh;

            if (mesh != null)
            {
                int blendShapeIndex =
                    mesh.GetBlendShapeIndex("Endocytosis");

                if (blendShapeIndex >= 0)
                {
                    float startWeight =
                        targetCell.GetBlendShapeWeight(blendShapeIndex);

                    elapsedTime = 0f;

                    while (elapsedTime < endocytosisResetDuration)
                    {
                        if (targetCell == null)
                            break;

                        elapsedTime += Time.deltaTime;

                        float progress = endocytosisResetDuration > 0f
                            ? Mathf.Clamp01(
                                elapsedTime / endocytosisResetDuration)
                            : 1f;

                        float weight = Mathf.Lerp(
                            startWeight,
                            0f,
                            progress
                        );

                        targetCell.SetBlendShapeWeight(
                            blendShapeIndex,
                            weight
                        );

                        yield return null;
                    }

                    if (targetCell != null)
                    {
                        targetCell.SetBlendShapeWeight(
                            blendShapeIndex,
                            0f
                        );
                    }
                }
            }
        }

        Debug.Log("NPC has been absorbed into the epithelial cell.");

        Destroy(gameObject);
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
            if (targetCell == null)
                yield break;

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

        if (targetCell != null)
        {
            targetCell.SetBlendShapeWeight(
                blendShapeIndex,
                endocytosisTargetWeight
            );
        }
    }
}
