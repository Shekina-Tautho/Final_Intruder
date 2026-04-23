using UnityEngine;
using UnityEngine.InputSystem;

public class InfectionInputVR : MonoBehaviour
{
    public InputActionProperty infectAction;

    void Update()
    {
        float value = infectAction.action.ReadValue<float>();

        if (value > 0.8f)
        {
            TryInfectNearby();
        }
    }

    void TryInfectNearby()
    {
        CellInfect[] cells = FindObjectsOfType<CellInfect>();

        foreach (var cell in cells)
        {
            cell.TryInfect();
        }
    }
}