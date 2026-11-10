
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

    [Header("Spread Movement")]
    [Tooltip("How long to wait before spreading begins.")]
    public float spreadStartDelay = 0.5f;

    [Tooltip("Controls how quickly spreading progresses. Lower values make it more gradual.")]
    [Range(0.1f, 2f)]
    public float spreadSpeedMultiplier = 0.65f;

    [Header("Random Diffusion")]
    public float randomStrength = 1.5f;
    public float directionChangeSpeed = 1f;

    [Header("Movement Transition")]
    [Tooltip("How long to smoothly blend from release movement into drifting.")]
    public float driftTransitionDuration = 2f;

    private Vector3 releaseDirection;
    private Vector3 startingPosition;
    private Vector3 randomDirection;
    private Vector3 spreadOffset;

    private Vector3 releaseVelocity;
    private Vector3 driftStartVelocity;

    private float releaseDistance;
    private float releaseDuration;
    private float releaseTimer;
    private float driftTransitionTimer;

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
        driftTransitionTimer = 0f;

        releaseVelocity = Vector3.zero;
        driftStartVelocity = Vector3.zero;

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
        float deltaTime = Time.deltaTime;

        if (isReleasing)
        {
            releaseTimer += deltaTime;

            // Existing synchronized rising/forward movement.
            float progress = ReleaseMovementProfile.GetProgress(
                releaseTimer,
                releaseDuration
            );

            Vector3 forwardMovement =
                releaseDirection *
                releaseDistance *
                progress;

            // Independent, slower spreading movement.
            float spreadElapsed = Mathf.Max(
                0f,
                releaseTimer - spreadStartDelay
            );

            float spreadDuration = Mathf.Max(
                0.01f,
                releaseDuration - spreadStartDelay
            );

            float spreadProgress = Mathf.Clamp01(
                spreadElapsed / spreadDuration
            );

            spreadProgress = Mathf.Pow(
                spreadProgress,
                1f / Mathf.Max(0.01f, spreadSpeedMultiplier)
            );

            Vector3 lateralSpread =
                spreadOffset * spreadProgress;

            // Calculate the new release position.
            Vector3 newPosition =
                startingPosition +
                forwardMovement +
                lateralSpread;

            // Track actual release velocity for a smooth transition.
            if (deltaTime > 0f)
            {
                releaseVelocity =
                    (newPosition - transform.position) / deltaTime;
            }

            transform.position = newPosition;

            // Begin drifting without resetting movement velocity.
            if (releaseTimer >= releaseDuration)
            {
                isReleasing = false;
                hasStartedDrifting = true;

                driftStartVelocity = releaseVelocity;
                driftTransitionTimer = 0f;

                Debug.Log(
                    "[RELEASE VIRUS] Smooth drift transition started: " +
                    gameObject.name
                );
            }

            return;
        }

        // Random diffusion behavior.
        if (hasStartedDrifting)
        {
            Vector3 targetDirection =
                Random.insideUnitSphere.normalized;

            randomDirection = Vector3.Slerp(
                randomDirection,
                targetDirection,
                directionChangeSpeed * deltaTime
            );

            Vector3 targetMovement =
                Vector3.back * driftSpeed +
                randomDirection * randomStrength;

            // Smoothly blend release velocity into normal drifting.
            if (driftTransitionTimer < driftTransitionDuration)
            {
                driftTransitionTimer += deltaTime;

                float transitionProgress = Mathf.Clamp01(
                    driftTransitionTimer /
                    Mathf.Max(0.01f, driftTransitionDuration)
                );

                // SmoothStep prevents an abrupt velocity change.
                float smoothProgress = Mathf.SmoothStep(
                    0f,
                    1f,
                    transitionProgress
                );

                Vector3 blendedMovement = Vector3.Lerp(
                    driftStartVelocity,
                    targetMovement,
                    smoothProgress
                );

                transform.position += blendedMovement * deltaTime;
            }
            else
            {
                // Normal random diffusion after the transition.
                transform.position += targetMovement * deltaTime;
            }
        }
    }
}


// Shared movement calculation for both player and release NPCs.
public static class ReleaseMovementProfile
{
    public static float fastPhaseDuration = 2f;
    public static float fastSpeedMultiplier = 1.6f;
    public static float slowSpeedMultiplier = 0.55f;

    public static void Configure(
        float fastDuration,
        float fastMultiplier,
        float slowMultiplier)
    {
        fastPhaseDuration = Mathf.Max(0f, fastDuration);
        fastSpeedMultiplier = Mathf.Max(0.01f, fastMultiplier);
        slowSpeedMultiplier = Mathf.Max(0.01f, slowMultiplier);
    }

    public static float GetProgress(
        float elapsed,
        float totalDuration)
    {
        totalDuration = Mathf.Max(0.01f, totalDuration);

        elapsed = Mathf.Clamp(
            elapsed,
            0f,
            totalDuration
        );

        float fastDuration = Mathf.Min(
            fastPhaseDuration,
            totalDuration
        );

        // If the entire release is within the fast phase.
        if (totalDuration <= fastDuration)
        {
            return elapsed / totalDuration;
        }

        // Calculate the total weighted movement.
        float fastMovement =
            fastDuration * fastSpeedMultiplier;

        float slowDuration =
            totalDuration - fastDuration;

        float slowMovement =
            slowDuration * slowSpeedMultiplier;

        float totalMovement =
            fastMovement + slowMovement;

        float currentMovement;

        if (elapsed <= fastDuration)
        {
            currentMovement =
                elapsed * fastSpeedMultiplier;
        }
        else
        {
            currentMovement =
                fastMovement +
                (elapsed - fastDuration) * slowSpeedMultiplier;
        }

        return Mathf.Clamp01(
            currentMovement / totalMovement
        );
    }
}
