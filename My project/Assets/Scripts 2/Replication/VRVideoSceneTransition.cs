using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VRVideoSceneTransition : MonoBehaviour
{
    public Image fadeImage;
    public VideoPlayer videoPlayer;

    public float fadeDuration = 2f;
    public string nextSceneName;

    private bool transitionStarted = false;

    private void Start()
    {
        if (fadeImage == null || videoPlayer == null)
        {
            Debug.LogError("Missing Fade Image or Video Player!");
            return;
        }

        // Start with a black screen
        SetAlpha(1f);

        // Listen for video completion
        videoPlayer.loopPointReached += OnVideoFinished;

        // Fade in
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            SetAlpha(1f - Mathf.Clamp01(elapsed / fadeDuration));

            yield return null;
        }

        SetAlpha(0f);
    }

    private void OnVideoFinished(VideoPlayer source)
    {
        if (transitionStarted)
            return;

        transitionStarted = true;
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            SetAlpha(Mathf.Clamp01(elapsed / fadeDuration));

            yield return null;
        }

        SetAlpha(1f);

        SceneManager.LoadScene(nextSceneName);
    }

    private void SetAlpha(float alpha)
    {
        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoFinished;
    }
}