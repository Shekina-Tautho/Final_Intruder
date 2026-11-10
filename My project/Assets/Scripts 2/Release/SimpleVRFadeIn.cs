using System.Collections;
using UnityEngine;

public class SimpleVRFadeIn : MonoBehaviour
{
    [Header("Fade Settings")]
    [Tooltip("How long the screen stays completely black.")]
    public float startDelay = 2f;

    [Tooltip("Duration of the fade-in.")]
    public float fadeDuration = 2f;

    private CanvasGroup canvasGroup;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 1f;
    }

    private void Start()
    {
        BeginFadeIn();
    }

    public void BeginFadeIn()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        if (startDelay > 0f)
        {
            yield return new WaitForSecondsRealtime(startDelay);
        }

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(
                elapsed / Mathf.Max(0.01f, fadeDuration)
            );

            canvasGroup.alpha = 1f - progress;

            yield return null;
        }

        canvasGroup.alpha = 0f;

        fadeCoroutine = null;
        gameObject.SetActive(false);
    }
}