
using UnityEngine;

public class ReleaseVirusMovement : MonoBehaviour
{
    [Header("Synchronized Release")]
    public float driftSpeed = 4f;

    [Header("Random Diffusion")]
    public float randomStrength = 1.5f;
    public float directionChangeSpeed = 1f;

    private Vector3 releaseDirection;
    private Vector3 startingPosition;
    private Vector3 randomDirection;

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
        float duration)
    {
        releaseDirection = direction.normalized;

        startingPosition = transform.position;

        releaseDistance = distance;
        releaseDuration = Mathf.Max(0.01f, duration);

        releaseTimer = 0f;

        isReleasing = true;
        hasStartedDrifting = false;

        Debug.Log(
            "[RELEASE VIRUS] Beginning synchronized release: " +
            gameObject.name
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

            float smoothProgress =
                Mathf.SmoothStep(0f, 1f, progress);

            transform.position = startingPosition +
                releaseDirection *
                releaseDistance *
                smoothProgress;

            if (progress >= 1f)
            {
                isReleasing = false;
                hasStartedDrifting = true;
            }

            return;
        }

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
