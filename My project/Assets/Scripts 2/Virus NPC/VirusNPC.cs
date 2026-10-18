using UnityEngine;

public class VirusNPC : MonoBehaviour
{
    [Header("Flow Movement")]
    public float flowSpeed = 3f;

    [Header("Random Diffusion")]
    public float randomStrength = 1.5f;
    public float directionChangeSpeed = 2f;

    private Vector3 randomDirection;

    void Start()
    {
        randomDirection = Random.insideUnitSphere;
    }

    void Update()
    {
        // Main mucus/fluid flow toward clearance
        Vector3 flowMovement = Vector3.back * flowSpeed;

        // Create a new random direction
        Vector3 targetDirection = Random.insideUnitSphere;

        // Smoothly change the random direction
        randomDirection = Vector3.Slerp(
            randomDirection,
            targetDirection,
            directionChangeSpeed * Time.deltaTime
        );

        // Combine fluid flow and diffusion
        Vector3 movement =
            flowMovement +
            randomDirection * randomStrength;

        transform.position += movement * Time.deltaTime;
    }
}