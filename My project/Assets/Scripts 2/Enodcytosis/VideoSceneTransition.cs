
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class VideoSceneTransition : MonoBehaviour
{
    [Header("Video")]
    public VideoPlayer videoPlayer;

    [Header("Fade to Black")]
    public GameObject blackScreen;
    public float fadeDuration = 1.5f;
    public float blackScreenDelay = 1.5f;

    [Header("Scene Transition")]
    public string nextSceneName;

    private Image blackScreenImage;
    private bool hasFinished = false;

    private void Start()
    {
        if (videoPlayer == null)
        {
            Debug.LogWarning("Video Player is not assigned.");
            return;
        }

        if (blackScreen != null)
        {
            blackScreenImage = blackScreen.GetComponent<Image>();

            if (blackScreenImage != null)
            {
                Color color = blackScreenImage.color;
                color.a = 0f;
                blackScreenImage.color = color;
            }

            blackScreen.SetActive(false);
        }

        videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void OnVideoFinished(VideoPlayer player)
    {
        if (hasFinished)
            return;

        hasFinished = true;
        StartCoroutine(FadeAndLoadScene());
    }

    private IEnumerator FadeAndLoadScene()
    {
        if (blackScreen != null && blackScreenImage != null)
        {
            blackScreen.SetActive(true);

            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;

                float alpha = Mathf.Clamp01(elapsed / fadeDuration);

                Color color = blackScreenImage.color;
                color.a = alpha;
                blackScreenImage.color = color;

                yield return null;
            }

            Color finalColor = blackScreenImage.color;
            finalColor.a = 1f;
            blackScreenImage.color = finalColor;
        }

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

    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }
}
