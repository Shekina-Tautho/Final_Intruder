using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AbsorptionFade : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 3f;

    private Coroutine fadeCoroutine;

    void Awake()
    {
        if (fadeImage == null)
            fadeImage = GetComponent<Image>();

        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = 0f;
            fadeImage.color = color;
        }
    }

    public void FadeToBlack()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        if (fadeImage == null)
            yield break;

        Color color = fadeImage.color;
        float startAlpha = color.a;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float progress = fadeDuration > 0f
                ? Mathf.Clamp01(elapsed / fadeDuration)
                : 1f;

            color.a = Mathf.Lerp(startAlpha, 1f, progress);
            fadeImage.color = color;

            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;
        fadeCoroutine = null;
    }

    void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current != null &&
            UnityEngine.InputSystem.Keyboard.current.bKey.wasPressedThisFrame)
        {
            FadeToBlack();
        }
    }
}
