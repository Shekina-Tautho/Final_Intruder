using UnityEngine;

public class ATPCollectible : MonoBehaviour
{
    public float staminaRestoreAmount = 30f;

    [Header("VFX")]
    public GameObject burstVFX;

    private Transform player;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        player = other.transform;

        PlayerStats stats = other.GetComponentInParent<PlayerStats>();

        if (stats != null)
        {
            stats.AddStamina(staminaRestoreAmount);
        }

        PlayBurst();
        StartCoroutine(AttractParticles());

        Destroy(gameObject, 0.1f);
    }

    void PlayBurst()
    {
        if (burstVFX != null)
        {
            Instantiate(burstVFX, transform.position, Quaternion.identity);
        }
    }

    System.Collections.IEnumerator AttractParticles()
    {
        float t = 0;
        float duration = 0.3f;

        while (t < duration)
        {
            t += Time.deltaTime;

            // pull object slightly toward player (illusion effect)
            transform.position = Vector3.Lerp(
                transform.position,
                player.position,
                t / duration
            );

            yield return null;
        }
    }
}