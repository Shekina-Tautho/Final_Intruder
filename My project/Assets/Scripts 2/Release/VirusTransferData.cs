
using System.Collections.Generic;
using UnityEngine;

public static class VirusTransferData
{
    // Existing player absorption data.
    public static Vector3 absorbedCellLocalPosition;
    public static bool hasAbsorbedCell = false;

    // stores the positions of epithelial cells
    // that have absorbed NPC viruses.
    public static List<Vector3> npcAbsorbedCellLocalPositions =
        new List<Vector3>();

    public static void RecordNPCAbsorbedCell(
        Transform epithelialCellsRoot,
        Vector3 worldPosition)
    {
        if (epithelialCellsRoot == null)
        {
            Debug.LogWarning(
                "VirusTransferData: Epithelial Cells Root is missing."
            );
            return;
        }

        Vector3 localPosition =
            epithelialCellsRoot.InverseTransformPoint(worldPosition);

        // Prevent the same epithelial cell from being recorded twice.
        foreach (Vector3 savedPosition in npcAbsorbedCellLocalPositions)
        {
            if (Vector3.Distance(savedPosition, localPosition) < 0.05f)
            {
                Debug.Log(
                    "NPC absorption already recorded for this cell."
                );
                return;
            }
        }

        npcAbsorbedCellLocalPositions.Add(localPosition);

        Debug.Log(
            "NPC-absorbed epithelial cell recorded: " + localPosition +
            "\nTotal recorded NPC-absorbed cells: " +
            npcAbsorbedCellLocalPositions.Count
        );
    }
}
