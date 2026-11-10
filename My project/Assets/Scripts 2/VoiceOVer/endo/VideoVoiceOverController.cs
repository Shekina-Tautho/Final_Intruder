
using UnityEngine;
using UnityEngine.Video;
using System;

public class VideoVoiceOverController : MonoBehaviour
{
    [Serializable]
    public class VoiceOverEntry
    {
        [Tooltip("Time in seconds when this voice-over should play.")]
        public float timestamp;

        public AudioClip audioClip;
    }

    [Header("Video Reference")]
    public VideoPlayer videoPlayer;

    [Header("Audio Settings")]
    public AudioSource voiceOverAudioSource;

    [Header("Voice-Over Clips")]
    public VoiceOverEntry[] voiceOvers;

    private bool[] hasPlayed;

    private void Awake()
    {
        if (voiceOvers != null)
        {
            hasPlayed = new bool[voiceOvers.Length];
        }
    }

    private void Update()
    {
        if (videoPlayer == null || voiceOverAudioSource == null)
            return;

        // Wait until the video is actually playing.
        if (!videoPlayer.isPlaying)
            return;

        // Check every voice-over entry.
        for (int i = 0; i < voiceOvers.Length; i++)
        {
            if (hasPlayed[i])
                continue;

            if (voiceOvers[i].audioClip == null)
                continue;

            // Play when the video reaches the assigned timestamp.
            if (videoPlayer.time >= voiceOvers[i].timestamp)
            {
                voiceOverAudioSource.PlayOneShot(
                    voiceOvers[i].audioClip
                );

                hasPlayed[i] = true;
            }
        }
    }
}
