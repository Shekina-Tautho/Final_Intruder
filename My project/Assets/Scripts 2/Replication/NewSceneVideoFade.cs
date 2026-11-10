using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class NewSceneVideoFade : MonoBehaviour
{
    [Header("References")]
    public Image fadeImage;
    public VideoPlayer videoPlayer;

    [Header("Fade Settings")]
    public float fadeInDuration = 2f;
    public float fadeOutDuration = 2f;

    [Header("Video Settings")]
    public float fadeOutStartSeconds = 10f;

    [Header("Next Scene")]
    public string nextSceneName;

    private bool isFadingOut = false;

    private void Start()
    {
        if (fadeImage == null || videoPlayer == null)
        {
            Debug.LogError("Missing Fade Image or Video Player!");
            return;
        }

        // Start with a black screen
        SetAlpha(1f);

        // Start the video
        videoPlayer.Play();

        // Fade in
        StartCoroutine(FadeIn());
    }

    private void Update()
    {
        if (isFadingOut || videoPlayer == null)
            return;

        if (videoPlayer.isPlaying &&
            videoPlayer.time >= fadeOutStartSeconds)
        {
            isFadingOut = true;
            StartCoroutine(FadeOutAndLoadScene());
        }
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            SetAlpha(1f - Mathf.Clamp01(
                elapsed / fadeInDuration
            ));

            yield return null;
        }

        SetAlpha(0f);
    }

    private IEnumerator FadeOutAndLoadScene()
    {
        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            SetAlpha(Mathf.Clamp01(
                elapsed / fadeOutDuration
            ));

            yield return null;
        }

        SetAlpha(1f);

        videoPlayer.Stop();

        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogError("Next scene name is empty!");
            yield break;
        }

        SceneManager.LoadScene(nextSceneName);
    }

    private void SetAlpha(float alpha)
    {
        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }
}