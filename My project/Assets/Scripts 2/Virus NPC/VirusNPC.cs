
using UnityEngine;

public class VirusNPC : MonoBehaviour
{
    [Header("Flow Movement")]
    public float flowSpeed = 3f;

    [Header("Random Diffusion")]
    public float randomStrength = 1.5f;
    public float directionChangeSpeed = 2f;

    [Header("Release Burst")]
    public float releaseBurstSpeed = 5f;
    public float releaseBurstDuration = 1.5f;

    private Vector3 randomDirection;
    private Vector3 releaseDirection;
    private float releaseTimer;
    private bool isReleasing;

    void Start()
    {
        randomDirection = Random.insideUnitSphere;
    }

    // Called by VirusSpawner when a virus is released.
    public void BeginRelease(Vector3 direction)
    {
        releaseDirection = direction.normalized;
        releaseTimer = releaseBurstDuration;
        isReleasing = true;
    }

    void Update()
    {
        // Initial outward movement from the epithelial cell.
        Vector3 burstMovement = Vector3.zero;

        if (isReleasing && releaseTimer > 0f)
        {
            burstMovement = releaseDirection * releaseBurstSpeed;

            releaseTimer -= Time.deltaTime;

            if (releaseTimer <= 0f)
            {
                isReleasing = false;
            }
        }

        // Main mucus/fluid flow toward clearance.
        Vector3 flowMovement = Vector3.back * flowSpeed;

        // Create a new random direction.
        Vector3 targetDirection = Random.insideUnitSphere;

        // Smoothly change the random direction.
        randomDirection = Vector3.Slerp(
            randomDirection,
            targetDirection,
            directionChangeSpeed * Time.deltaTime
        );

        // Combine burst, fluid flow, and diffusion.
        Vector3 movement =
            burstMovement +
            flowMovement +
            randomDirection * randomStrength;

        transform.position += movement * Time.deltaTime;
    }
}
