using UnityEngine;

public class SceneFadeIn : MonoBehaviour
{
    [Header("Fade Settings")]
    public Renderer fadeRenderer;
    public float fadeDuration = 3f;

    private float timer = 0f;

    void Start()
    {
        SetFadeAlpha(1f);
    }

    void Update()
    {
        if (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            float alpha = 1f - Mathf.Clamp01(timer / fadeDuration);

            SetFadeAlpha(alpha);
        }
        else
        {
            SetFadeAlpha(0f);
        }
    }

    void SetFadeAlpha(float alpha)
    {
        if (fadeRenderer == null)
            return;

        Color color = fadeRenderer.material.color;
        color.a = alpha;
        fadeRenderer.material.color = color;
    }
}