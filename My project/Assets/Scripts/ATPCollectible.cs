using UnityEngine;

public class ATPCollectible : MonoBehaviour
{
    public float staminaRestoreAmount = 30f;

    [Header("VFX")]
    public GameObject burstVFX;

    [Header("SFX")]
    public AudioSource audioSource;
    public AudioClip collectClip;

    private Transform player;

    private void Awake()
    {
        // auto-assign if not manually set
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        player = other.transform;

        PlayerStats stats = other.GetComponentInParent<PlayerStats>();

        if (stats != null)
        {
            stats.AddStamina(staminaRestoreAmount);
        }

        PlayCollectSound();
        PlayBurst();

        // delay destroy slightly more safely
        Destroy(gameObject);
    }

    void PlayCollectSound()
    {
        if (collectClip == null) return;

        // Create temporary audio object (SAFE METHOD)
        GameObject audioObj = new GameObject("ATP_SFX");
        audioObj.transform.position = transform.position;

        AudioSource src = audioObj.AddComponent<AudioSource>();

        src.clip = collectClip;
        src.spatialBlend = 1f; // 3D sound
        src.minDistance = 1f;
        src.maxDistance = 15f;
        src.rolloffMode = AudioRolloffMode.Logarithmic;
        src.volume = 1f;

        src.Play();
        Debug.Log("ATP SOUND PLAYED");

        Destroy(audioObj, collectClip.length + 0.1f);
    }

    void PlayBurst()
    {
        if (burstVFX != null)
        {
            Instantiate(burstVFX, transform.position, Quaternion.identity);
        }
    }
}