
using UnityEngine;

public class ReleaseVirusMovement : MonoBehaviour
{
    [Header("Synchronized Release")]
    public float driftSpeed = 4f;

    [Header("Progressive Spreading")]
    [Tooltip("Maximum sideways spreading distance.")]
    public float spreadRadius = 3.5f;

    [Tooltip("Controls how much viruses spread vertically.")]
    [Range(0f, 1f)]
    public float verticalSpreadMultiplier = 0.4f;

    [Header("Random Diffusion")]
    public float randomStrength = 1.5f;
    public float directionChangeSpeed = 1f;

    private Vector3 releaseDirection;
    private Vector3 startingPosition;
    private Vector3 randomDirection;
    private Vector3 spreadOffset;

    private float releaseDistance;
    private float releaseDuration;
    private float releaseTimer;

    private bool isReleasing;
    private bool hasStartedDrifting;

    private void Start()
    {
        randomDirection = Random.insideUnitSphere.normalized;
    }

    public void BeginRelease(
        Vector3 direction,
        float distance,
        float duration,
        int virusIndex,
        int totalViruses)
    {
        releaseDirection = direction.normalized;

        startingPosition = transform.position;

        releaseDistance = distance;
        releaseDuration = Mathf.Max(0.01f, duration);

        releaseTimer = 0f;

        // Calculate directions perpendicular to forward movement.
        Vector3 right = Vector3.Cross(
            releaseDirection,
            Vector3.up
        ).normalized;

        if (right.sqrMagnitude < 0.001f)
        {
            right = Vector3.Cross(
                releaseDirection,
                Vector3.forward
            ).normalized;
        }

        Vector3 up = Vector3.Cross(
            right,
            releaseDirection
        ).normalized;

        // Distribute viruses using a golden-angle pattern.
        float goldenAngle = 137.5f;

        float angle = virusIndex * goldenAngle;

        float normalizedRadius = Mathf.Sqrt(
            (virusIndex + 0.5f) /
            Mathf.Max(1, totalViruses)
        );

        float radius = normalizedRadius * spreadRadius;

        float angleRadians = angle * Mathf.Deg2Rad;

        float horizontal = Mathf.Cos(angleRadians) * radius;
        float vertical = Mathf.Sin(angleRadians) * radius;

        spreadOffset =
            right * horizontal +
            up * vertical * verticalSpreadMultiplier;

        isReleasing = true;
        hasStartedDrifting = false;

        Debug.Log(
            "[RELEASE VIRUS] Beginning synchronized release: " +
            gameObject.name +
            " | Spread offset: " + spreadOffset
        );
    }

    private void Update()
    {
        if (isReleasing)
        {
            releaseTimer += Time.deltaTime;

            float progress = Mathf.Clamp01(
                releaseTimer / releaseDuration
            );

            float smoothProgress = Mathf.SmoothStep(
                0f,
                1f,
                progress
            );

            // Synchronized forward movement.
            Vector3 forwardMovement =
                releaseDirection *
                releaseDistance *
                smoothProgress;

            // Progressive spreading WHILE moving.
            Vector3 lateralSpread =
                spreadOffset * smoothProgress;

            transform.position =
                startingPosition +
                forwardMovement +
                lateralSpread;

            if (progress >= 1f)
            {
                isReleasing = false;
                hasStartedDrifting = true;
            }

            return;
        }

        // Existing random diffusion behavior.
        if (hasStartedDrifting)
        {
            Vector3 targetDirection =
                Random.insideUnitSphere.normalized;

            randomDirection = Vector3.Slerp(
                randomDirection,
                targetDirection,
                directionChangeSpeed * Time.deltaTime
            );

            Vector3 movement =
                Vector3.back * driftSpeed +
                randomDirection * randomStrength;

            transform.position += movement * Time.deltaTime;
        }
    }
}
