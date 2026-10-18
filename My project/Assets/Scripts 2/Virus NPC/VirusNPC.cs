
using UnityEngine;

public class VirusNPC : MonoBehaviour
{
    [Header("Flow Movement")]
    public float flowSpeed = 3f;

    [Header("Random Diffusion")]
    public float randomStrength = 2.5f;
    public float directionChangeSpeed = 1.5f;

    [Header("Sideways Movement")]
    public float sidewaysStrength = 2f;

    private Vector3 randomDirection;

    void Start()
    {
        randomDirection = Random.insideUnitSphere;
    }

    void Update()
    {
        // Main flow toward clearance
        Vector3 flowMovement = Vector3.back * flowSpeed;

        // Random movement in all directions
        Vector3 targetDirection = Random.insideUnitSphere;

        randomDirection = Vector3.Slerp(
            randomDirection,
            targetDirection,
            directionChangeSpeed * Time.deltaTime
        );

        // Encourage movement toward the airway's sides
        Vector3 sidewaysMovement = Vector3.right *
            Random.Range(-sidewaysStrength, sidewaysStrength);

        // Combine flow, diffusion, and sideways drift
        Vector3 movement =
            flowMovement +
            randomDirection * randomStrength +
            sidewaysMovement;

        transform.position += movement * Time.deltaTime;
    }
}
