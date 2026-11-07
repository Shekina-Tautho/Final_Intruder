
using UnityEngine;

public class ModelRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 15f;

    void Update()
    {
        transform.Rotate(
            Vector3.up,
            rotationSpeed * Time.deltaTime,
            Space.Self
        );
    }
}
