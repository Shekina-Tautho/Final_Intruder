
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class VideoSceneTransition : MonoBehaviour
{
    [Header("Video")]
    public VideoPlayer videoPlayer;
    public double videoStartTime = 3.0;

    [Header("Fade Settings")]
    public GameObject blackScreen;
    public float fadeInDuration = 1.5f;
    public float fadeOutDuration = 1.5f;
    public float blackScreenDelay = 1.5f;

    [Header("Scene Transition")]
    public string nextSceneName;

    private Image blackScreenImage;
    private bool hasFinished = false;

    private void Start()
    {
        if (videoPlayer == null || blackScreen == null)
        {
            Debug.LogWarning("Video Player or Black Screen is not assigned.");
            return;
        }

        blackScreenImage = blackScreen.GetComponent<Image>();

        if (blackScreenImage == null)
        {
            Debug.LogWarning("BlackScreen needs an Image component.");
            return;
        }

        // Start with a completely black screen.
        Color color = blackScreenImage.color;
        color.a = 1f;
        blackScreenImage.color = color;

        blackScreen.SetActive(true);

        // Prevent the video from playing automatically.
        videoPlayer.playOnAwake = false;

        // Listen for video events.
        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.prepareCompleted += OnVideoPrepared;

        // Prepare the video before playback.
        videoPlayer.Prepare();
    }

    private void OnVideoPrepared(VideoPlayer player)
    {
        // Remove listener after preparation.
        videoPlayer.prepareCompleted -= OnVideoPrepared;

        // Skip the first 3 seconds.
        videoPlayer.time = videoStartTime;

        // Wait until seeking is complete.
        StartCoroutine(StartVideoAtPosition());
    }

    private IEnumerator StartVideoAtPosition()
    {
        yield return new WaitUntil(() =>
            videoPlayer.isPrepared &&
            videoPlayer.time >= videoStartTime - 0.05
        );

        // Start the video from the requested timestamp.
        videoPlayer.Play();

        // Fade in and reveal the video.
        yield return StartCoroutine(Fade(1f, 0f, fadeInDuration));

        blackScreen.SetActive(false);
    }

    private void OnVideoFinished(VideoPlayer player)
    {
        if (hasFinished)
            return;

        hasFinished = true;
        StartCoroutine(FadeOutAndLoadScene());
    }

    private IEnumerator FadeOutAndLoadScene()
    {
        blackScreen.SetActive(true);

        yield return StartCoroutine(
            Fade(0f, 1f, fadeOutDuration)
        );

        // Keep the screen black before loading the next scene.
        yield return new WaitForSecondsRealtime(blackScreenDelay);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("Next scene name is empty.");
        }
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;

        if (duration <= 0f)
        {
            Color immediateColor = blackScreenImage.color;
            immediateColor.a = endAlpha;
            blackScreenImage.color = immediateColor;
            yield break;
        }

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsed / duration);
            float alpha = Mathf.Lerp(startAlpha, endAlpha, progress);

            Color color = blackScreenImage.color;
            color.a = alpha;
            blackScreenImage.color = color;

            yield return null;
        }

        Color finalColor = blackScreenImage.color;
        finalColor.a = endAlpha;
        blackScreenImage.color = finalColor;
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
            videoPlayer.prepareCompleted -= OnVideoPrepared;
        }
    }
}
