
using UnityEngine;
using UnityEngine.Video;

public class VoiceOverController : MonoBehaviour
{
    [System.Serializable]
    public class VoiceOverEntry
    {
        [Tooltip("Video timestamp when this voice-over should play.")]
        public float timestamp;

        [Tooltip("Voice-over audio clip.")]
        public AudioClip audioClip;
    }

    [Header("Video")]
    public VideoPlayer videoPlayer;

    [Header("Voice Over")]
    public AudioSource voiceOverSource;

    [Header("Voice Over Entries")]
    public VoiceOverEntry[] voiceOvers;

    private bool[] hasPlayed;

    private void Start()
    {
        hasPlayed = new bool[voiceOvers.Length];
    }

    private void Update()
    {
        if (videoPlayer == null || voiceOverSource == null)
            return;

        if (!videoPlayer.isPlaying)
            return;

        for (int i = 0; i < voiceOvers.Length; i++)
        {
            if (hasPlayed[i])
                continue;

            if (voiceOvers[i].audioClip == null)
                continue;

            if (videoPlayer.time >= voiceOvers[i].timestamp)
            {
                voiceOverSource.PlayOneShot(voiceOvers[i].audioClip);
                hasPlayed[i] = true;
            }
        }
    }

    public void StopVoiceOver()
    {
        if (voiceOverSource != null)
        {
            voiceOverSource.Stop();
        }
    }
}
