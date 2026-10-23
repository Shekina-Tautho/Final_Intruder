using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class AbsorptionFade : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 3f;

    private Coroutine fadeCoroutine;

    [Header("Scene Transition")]
    public string nextSceneName;
    public float waitBeforeLoading = 0.5f;

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

    // Keep the screen black before loading the next scene.
    yield return new WaitForSecondsRealtime(waitBeforeLoading);

    if (!string.IsNullOrEmpty(nextSceneName))
    {
        SceneManager.LoadScene(nextSceneName);
    }
    else
    {
        Debug.LogWarning("AbsorptionFade: Next scene name is empty.");
    }

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
