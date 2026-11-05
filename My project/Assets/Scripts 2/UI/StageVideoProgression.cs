
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

    [Header("Background Music")]
    public AudioSource backgroundMusicSource;

    [Header("Stage Transition Sound")]
    public AudioSource sfxAudioSource;
    public AudioClip stageChangeSound;
    [Range(0f, 1f)]
    public float stageChangeVolume = 1f;

    [Header("Ending Fade")]
    public Renderer fadeRenderer;
    public float fadeStartTime = 5f;
    public float fadeDuration = 5f;

    [Header("Scene Transition")]
    public string nextSceneName = "SampleScene";

    private bool sceneTransitionStarted = false;
    private bool musicStarted = false;

    private int currentStage = 0;
    private float originalMusicVolume;

    void Awake()
    {
        if (videoPlayer != null)
        {
            videoPlayer.started += OnVideoStarted;
        }
    }

    void Start()
    {
        SetStage(1);
        SetFadeAlpha(0f);

        if (backgroundMusicSource != null)
        {
            originalMusicVolume = backgroundMusicSource.volume;
            backgroundMusicSource.playOnAwake = false;
            backgroundMusicSource.loop = true;
        }

        // Handle videos that are already playing
        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            StartBackgroundMusic();
        }
    }

    private void OnVideoStarted(VideoPlayer source)
    {
        StartBackgroundMusic();
    }

    private void StartBackgroundMusic()
    {
        if (musicStarted || backgroundMusicSource == null)
            return;

        musicStarted = true;

        backgroundMusicSource.volume = originalMusicVolume;
        backgroundMusicSource.Play();
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

            // Fade background music together with the screen
            if (backgroundMusicSource != null && musicStarted)
            {
                backgroundMusicSource.volume =
                    Mathf.Lerp(originalMusicVolume, 0f, alpha);
            }

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
        if (stage < 1 || stage > 2)
            return;

        // Avoid triggering the same stage twice
        if (currentStage == stage)
            return;

        currentStage = stage;

        // Exposure appearance
        if (exposureNormal != null)
            exposureNormal.gameObject.SetActive(stage != 1);

        if (exposureGlow != null)
            exposureGlow.gameObject.SetActive(stage == 1);

        // Tract Entry appearance
        if (tractEntryNormal != null)
            tractEntryNormal.gameObject.SetActive(stage != 2);

        if (tractEntryGlow != null)
            tractEntryGlow.gameObject.SetActive(stage == 2);

        // Update stage information
        if (stage == 1)
        {
            if (stageTitle != null)
                stageTitle.text = "EXPOSURE";

            if (stageDescription != null)
                stageDescription.text = exposureDescription;

            if (stageNumber != null)
                stageNumber.text = "01 / 10";
        }
        else if (stage == 2)
        {
            if (stageTitle != null)
                stageTitle.text = "TRACT ENTRY";

            if (stageDescription != null)
                stageDescription.text = tractEntryDescription;

            if (stageNumber != null)
                stageNumber.text = "02 / 10";

            // Play sound when the stage UI changes
            if (sfxAudioSource != null && stageChangeSound != null)
            {
                sfxAudioSource.PlayOneShot(
                    stageChangeSound,
                    stageChangeVolume
                );
            }
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

    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.started -= OnVideoStarted;
        }
    }
}
