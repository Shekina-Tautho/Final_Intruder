
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
        Quit
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

    private AudioSource audioSource;
    private Button button;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        button = GetComponent<Button>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

        if (button != null)
            button.onClick.AddListener(HandleClick);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSound != null)
            audioSource.PlayOneShot(hoverSound, hoverVolume);
    }

    private void HandleClick()
    {
        StartCoroutine(PlaySoundThenAct());
    }

    private IEnumerator PlaySoundThenAct()
    {
        if (clickSound != null)
        {
            audioSource.PlayOneShot(clickSound, clickVolume);
            yield return new WaitForSeconds(clickSound.length);
        }

        if (buttonAction == ButtonAction.LoadScene)
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else if (buttonAction == ButtonAction.Quit)
        {
            Application.Quit();
        }
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(HandleClick);
    }
}
