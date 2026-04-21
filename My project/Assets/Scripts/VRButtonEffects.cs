using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;

public class VRButtonEffects : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler
{
    [Header("Scale Settings")]
    public Vector3 normalScale = Vector3.one;
    public Vector3 hoverScale = Vector3.one * 1.08f;
    public Vector3 pressedScale = Vector3.one * 0.95f;

    [Header("Animation Speed")]
    public float smooth = 12f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip hoverSound;
    public AudioClip clickSound;

    private Vector3 targetScale;

    void Start()
    {
        targetScale = normalScale;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.deltaTime * smooth
        );
    }

    // 👀 HOVER ENTER
    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = hoverScale;

        if (audioSource && hoverSound)
            audioSource.PlayOneShot(hoverSound, 1.5f);
    }

    // 👀 HOVER EXIT
    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = normalScale;
    }

    // 👆 PRESS DOWN
    public void OnPointerDown(PointerEventData eventData)
    {
        targetScale = pressedScale;

        if (audioSource && clickSound)
            audioSource.PlayOneShot(clickSound);

        SendHaptics(eventData);
    }

    // 👆 RELEASE
    public void OnPointerUp(PointerEventData eventData)
    {
        targetScale = hoverScale;
    }

    // ✋ HAPTICS FUNCTION
    void SendHaptics(PointerEventData eventData)
    {
        var interactable = eventData.pointerPressRaycast.gameObject;

        if (interactable == null) return;

        var controller = FindObjectOfType<ActionBasedController>();

        if (controller != null)
        {
            controller.SendHapticImpulse(0.2f, 0.1f);
        }
    }
}