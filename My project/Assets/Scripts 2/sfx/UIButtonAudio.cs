using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class UIButtonAudio : MonoBehaviour, IPointerEnterHandler
{
    public enum ButtonAction
    {
        None,
        LoadScene,
        Quit,
        OpenDisclaimer
    }

    [Header("Button Sounds")]
    public AudioClip hoverSound;
    public AudioClip clickSound;

    [Header("Sound Volumes")]
    [Range(0f, 2f)]
    public float hoverVolume = 0.5f;

    [Range(0f, 2f)]
    public float clickVolume = 1.5f;

    [Header("Button Action")]
    public ButtonAction buttonAction = ButtonAction.None;
    public string nextSceneName = "IntroductionScene";

    [Header("Disclaimer Reference")]
    public DisclaimerController disclaimerController;

    private AudioSource audioSource;
    private Button button;
    private Toggle toggle;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        button = GetComponent<Button>();
        toggle = GetComponent<Toggle>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

        if (button != null)
        {
            button.onClick.AddListener(HandleButtonClick);
        }

        if (toggle != null)
        {
            toggle.onValueChanged.AddListener(HandleToggleClick);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound, hoverVolume);
        }
    }

    private void HandleButtonClick()
    {
        // Play click sound immediately
        if (clickSound != null)
        {
            audioSource.PlayOneShot(clickSound, clickVolume);
        }

        // No additional action needed for Continue and X buttons
        if (buttonAction == ButtonAction.None)
            return;

        // Start coroutine only for actions that need a delay
        if (isActiveAndEnabled)
        {
            StartCoroutine(PlaySoundThenAct());
        }
    }

    private void HandleToggleClick(bool isOn)
    {
        if (clickSound != null)
        {
            audioSource.PlayOneShot(clickSound, clickVolume);
        }
    }

    private IEnumerator PlaySoundThenAct()
    {
        if (clickSound != null)
        {
            yield return new WaitForSeconds(clickSound.length);
        }

        switch (buttonAction)
        {
            case ButtonAction.OpenDisclaimer:

                if (disclaimerController != null)
                {
                    disclaimerController.OpenDisclaimer();
                }
                else
                {
                    Debug.LogWarning("Disclaimer Controller is not assigned.");
                }

                break;

            case ButtonAction.LoadScene:

                if (!string.IsNullOrEmpty(nextSceneName))
                {
                    SceneManager.LoadScene(nextSceneName);
                }
                else
                {
                    Debug.LogWarning("Next Scene Name is empty.");
                }

                break;

            case ButtonAction.Quit:

                Application.Quit();

                break;
        }
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(HandleButtonClick);
        }

        if (toggle != null)
        {
            toggle.onValueChanged.RemoveListener(HandleToggleClick);
        }
    }
}