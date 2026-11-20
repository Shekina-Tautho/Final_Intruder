
using UnityEngine;

public class ModelRotator : MonoBehaviour
{
    public enum RotationAxis
    {
        LocalX,
        LocalY,
        LocalZ
    }

    [Header("Rotation Settings")]
    public float rotationSpeed = 15f;

    [Header("Rotation Axis")]
    public RotationAxis rotationAxis = RotationAxis.LocalY;

    void Update()
    {
        Vector3 axis = Vector3.up;

        switch (rotationAxis)
        {
            case RotationAxis.LocalX:
                axis = Vector3.right;
                break;

            case RotationAxis.LocalY:
                axis = Vector3.up;
                break;

            case RotationAxis.LocalZ:
                axis = Vector3.forward;
                break;
        }

        transform.Rotate(
            axis,
            rotationSpeed * Time.deltaTime,
            Space.Self
        );
    }
}
