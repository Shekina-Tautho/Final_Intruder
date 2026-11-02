
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class UIButtonAudio : MonoBehaviour, IPointerEnterHandler
{
    [Header("Button Sounds")]
    public AudioClip hoverSound;
    public AudioClip clickSound;

    [Header("Sound Volumes")]
    [Range(0f, 2f)]
    public float hoverVolume = 0.5f;

    [Range(0f, 2f)]
    public float clickVolume = 1.5f;

    private AudioSource audioSource;
    private Button button;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        button = GetComponent<Button>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

        if (button != null)
            button.onClick.AddListener(PlayClickSound);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSound != null)
            audioSource.PlayOneShot(hoverSound, hoverVolume);
    }

    private void PlayClickSound()
    {
        if (clickSound != null)
            audioSource.PlayOneShot(clickSound, clickVolume);
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(PlayClickSound);
    }
}
