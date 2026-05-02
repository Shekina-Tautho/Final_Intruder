using UnityEngine;
using System.Collections;

public class BackgroundHeartbeat : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip heartbeatClip;

    [Header("Timing")]
    public float minInterval = 6f;
    public float maxInterval = 8f;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        StartCoroutine(HeartbeatLoop());
    }

    IEnumerator HeartbeatLoop()
    {
        while (true)
        {
            float waitTime = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(waitTime);

            PlayHeartbeat();
        }
    }

    void PlayHeartbeat()
    {
        if (heartbeatClip == null || audioSource == null) return;

        audioSource.pitch = Random.Range(0.97f, 1.03f); // subtle variation
        audioSource.PlayOneShot(heartbeatClip);
    }
}