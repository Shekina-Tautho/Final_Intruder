using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip backgroundMusic;

    [Range(0f, 1f)]
    public float volume = 0.4f;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        PlayMusic();
    }

    void PlayMusic()
    {
        if (backgroundMusic == null) return;

        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.volume = volume;
        audioSource.Play();
    }
}