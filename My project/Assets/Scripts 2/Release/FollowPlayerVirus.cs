
using UnityEngine;

public class FollowPlayerVirus : MonoBehaviour
{
    [Header("Following")]
    public float followSmoothness = 3f;

    private Transform playerTarget;
    private Vector3 localOffset;
    private Vector3 velocity;

    public void Initialize(Transform target)
    {
        playerTarget = target;

        // Remember this NPC's position relative to the player.
        localOffset = playerTarget.InverseTransformPoint(
            transform.position
        );
    }

    private void LateUpdate()
    {
        if (playerTarget == null)
            return;

        // Maintain the NPC's original formation around the player.
        Vector3 targetPosition =
            playerTarget.TransformPoint(localOffset);

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            1f / followSmoothness
        );
    }
}
