
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using UnityEngine.SceneManagement;

public class StageVideoProgression : MonoBehaviour
{
    [Header("Video Player")]
    public VideoPlayer videoPlayer;

    [Header("Stage Information")]
    public TMP_Text stageTitle;
    public TMP_Text stageDescription;
    public TMP_Text stageNumber;

    [Header("Exposure Images")]
    public Image exposureNormal;
    public Image exposureGlow;

    [Header("Tract Entry Images")]
    public Image tractEntryNormal;
    public Image tractEntryGlow;

    [Header("Stage Descriptions")]
    [TextArea(3, 5)]
    public string exposureDescription;

    [TextArea(3, 5)]
    public string tractEntryDescription;

    [Header("Stage Timestamps")]
    public double tractEntryStartTime = 12.0;

    [Header("Ending Fade")]
    public Renderer fadeRenderer;
    public float fadeStartTime = 5f;
    public float fadeDuration = 5f;

    [Header("Scene Transition")]
    public string nextSceneName = "SampleScene";

    private bool sceneTransitionStarted = false;

    private int currentStage = 0;

    void Start()
    {
        SetStage(1);
        SetFadeAlpha(0f);
    }

    void Update()
    {
        if (videoPlayer == null)
            return;

        // Stage progression
        if (videoPlayer.isPlaying)
        {
            if (currentStage == 1 &&
                videoPlayer.time >= tractEntryStartTime)
            {
                SetStage(2);
            }
        }

        // Ending fade
        if (videoPlayer.length > 0 && fadeDuration > 0)
        {
            double fadeStart = videoPlayer.length - fadeStartTime;
            double fadeEnd = fadeStart + fadeDuration;

            float alpha = Mathf.InverseLerp(
                (float)fadeStart,
                (float)fadeEnd,
                (float)videoPlayer.time
            );

            SetFadeAlpha(alpha);

            // Load the next scene after the screen becomes completely black
            if (alpha >= 1f && !sceneTransitionStarted)
            {
                sceneTransitionStarted = true;
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }

    void SetStage(int stage)
    {
        currentStage = stage;

        // Exposure appearance
        exposureNormal.gameObject.SetActive(stage != 1);
        exposureGlow.gameObject.SetActive(stage == 1);

        // Tract Entry appearance
        tractEntryNormal.gameObject.SetActive(stage != 2);
        tractEntryGlow.gameObject.SetActive(stage == 2);

        // Update stage information
        if (stage == 1)
        {
            stageTitle.text = "EXPOSURE";
            stageDescription.text = exposureDescription;
            stageNumber.text = "01 / 10";
        }
        else if (stage == 2)
        {
            stageTitle.text = "TRACT ENTRY";
            stageDescription.text = tractEntryDescription;
            stageNumber.text = "02 / 10";
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
