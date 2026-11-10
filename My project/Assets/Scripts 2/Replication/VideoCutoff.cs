using UnityEngine;
using UnityEngine.Video;

public class VideoCutoff : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public double stopAtSeconds = 12.0;

    private void Update()
    {
        if (videoPlayer != null &&
            videoPlayer.isPlaying &&
            videoPlayer.time >= stopAtSeconds)
        {
            videoPlayer.Stop();
        }
    }
}