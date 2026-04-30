using UnityEngine;

public class ATPCollectible : MonoBehaviour
{
    [Header("Stamina Reward")]
    public float staminaRestoreAmount = 30f;

    [Header("VFX (optional)")]
    public GameObject collectEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerStats player = other.GetComponentInParent<PlayerStats>();

        if (player != null)
        {
            player.AddStamina(staminaRestoreAmount);
        }

        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}