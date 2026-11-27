
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

// Explicit XR aliases to prevent naming conflicts.
using XRInputDevice = UnityEngine.XR.InputDevice;
using XRCommonUsages = UnityEngine.XR.CommonUsages;

public class SceneStartVoiceOver : MonoBehaviour
{
    [Header("Voice-Over Audio")]
    public AudioSource voiceOverAudioSource;

    [Header("Voice-Over Recordings")]
    public AudioClip firstVoiceOverClip;
    public AudioClip secondVoiceOverClip;

    [Header("Simulation UI")]
    public SimulationUIController simulationUIController;

    [Header("Macrophage Information Panel")]
    public GameObject macrophageInfoPanel;

    [Header("Panel Dismissal Audio")]
    public AudioSource buttonAudioSource;
    public AudioClip buttonClickSound;

    [Range(0f, 1f)]
    public float buttonClickVolume = 1f;

    [Header("Delay Settings")]
    public float startDelay = 1f;
    public float secondVoiceOverDelay = 2f;

    [Header("Stage Transition Settings")]
    [Min(0f)]
    public float nextStageTransitionTime = 17f;

    private bool macrophagePanelDismissalEnabled = false;
    private bool previousButtonState = false;

    private readonly List<XRInputDevice> controllerDevices =
        new List<XRInputDevice>();

    private void Awake()
    {
        // Keep the macrophage panel hidden when the scene starts.
        if (macrophageInfoPanel != null)
        {
            macrophageInfoPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning(
                "Macrophage Info Panel is not assigned!"
            );
        }
    }

    private IEnumerator Start()
    {
        // Wait after the scene starts.
        yield return new WaitForSeconds(startDelay);

        if (voiceOverAudioSource == null)
        {
            Debug.LogWarning("Voice-over AudioSource is missing!");
            yield break;
        }

        // Play the first recording.
        if (firstVoiceOverClip != null)
        {
            voiceOverAudioSource.PlayOneShot(firstVoiceOverClip);

            // Wait until the first recording finishes.
            yield return new WaitForSeconds(firstVoiceOverClip.length);
        }
        else
        {
            Debug.LogWarning("First voice-over clip is missing!");
            yield break;
        }

        // Calculate when to play the second voice-over.
        float transitionLeadTime = Mathf.Min(1f, secondVoiceOverDelay);

        // Wait until 1 second before the second voice-over.
        yield return new WaitForSeconds(
            secondVoiceOverDelay - transitionLeadTime
        );

        // Keep the simulation UI at Release.
        // No Spread stage transition is needed.

        // Wait 1 second before playing the second recording.
        yield return new WaitForSeconds(transitionLeadTime);

        // Play the second recording.
        if (secondVoiceOverClip != null)
        {
            voiceOverAudioSource.PlayOneShot(secondVoiceOverClip);

            Debug.Log("Second voice-over started. Stage remains at Release.");

            // Wait until the configured stage transition time.
            yield return new WaitForSeconds(nextStageTransitionTime);

            // Activate the last stage directly.
            if (simulationUIController != null)
            {
                simulationUIController.SetStage(8);

                Debug.Log(
                    "Last stage activated at the configured transition time."
                );
            }
            else
            {
                Debug.LogWarning("Simulation UI Controller is missing!");
            }

            // Wait for the remaining duration of the second voice-over.
            float remainingTime =
                secondVoiceOverClip.length - nextStageTransitionTime;

            if (remainingTime > 0f)
            {
                yield return new WaitForSeconds(remainingTime);
            }

            // The second voice-over has now finished.
            Debug.Log("Second voice-over finished.");

            // Show the macrophage information panel.
            ShowMacrophageInfoPanel();
        }
        else
        {
            Debug.LogWarning("Second voice-over clip is missing!");
        }
    }

    private void Update()
    {
        if (!macrophagePanelDismissalEnabled)
            return;

        // Check VR controller buttons.
        bool controllerPressed = IsAnyControllerButtonPressed();

        bool controllerJustPressed =
            controllerPressed && !previousButtonState;

        // Check keyboard Enter.
        bool keyboardPressed = false;

        if (Keyboard.current != null)
        {
            keyboardPressed =
                Keyboard.current.enterKey.wasPressedThisFrame ||
                Keyboard.current.numpadEnterKey.wasPressedThisFrame;
        }

        if (controllerJustPressed || keyboardPressed)
        {
            DismissMacrophageInfoPanel();
        }

        previousButtonState = controllerPressed;
    }

    private void ShowMacrophageInfoPanel()
    {
        if (macrophageInfoPanel == null)
        {
            Debug.LogWarning(
                "Cannot show macrophage panel. Panel is not assigned!"
            );
            return;
        }

        macrophageInfoPanel.SetActive(true);

        // Play button click sound when the panel appears.
        if (buttonAudioSource != null &&
            buttonClickSound != null)
        {
            buttonAudioSource.PlayOneShot(
                buttonClickSound,
                buttonClickVolume
            );
        }

        // Prevent a previously held button from dismissing the panel.
        previousButtonState = IsAnyControllerButtonPressed();

        // Enable dismissal only after the panel appears.
        macrophagePanelDismissalEnabled = true;

        Debug.Log("Macrophage information panel displayed.");
    }

    private void DismissMacrophageInfoPanel()
    {
        if (macrophageInfoPanel == null ||
            !macrophageInfoPanel.activeInHierarchy)
        {
            return;
        }

        macrophageInfoPanel.SetActive(false);

        macrophagePanelDismissalEnabled = false;

        // Play button click sound.
        if (buttonAudioSource != null &&
            buttonClickSound != null)
        {
            buttonAudioSource.PlayOneShot(
                buttonClickSound,
                buttonClickVolume
            );
        }

        Debug.Log("Macrophage information panel dismissed.");
    }

    private bool IsAnyControllerButtonPressed()
    {
        controllerDevices.Clear();

        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.Controller,
            controllerDevices
        );

        foreach (XRInputDevice device in controllerDevices)
        {
            if (IsPressed(device, XRCommonUsages.primaryButton) ||
                IsPressed(device, XRCommonUsages.secondaryButton) ||
                IsPressed(device, XRCommonUsages.triggerButton) ||
                IsPressed(device, XRCommonUsages.gripButton) ||
                IsPressed(device, XRCommonUsages.menuButton) ||
                IsPressed(device, XRCommonUsages.primary2DAxisClick) ||
                IsPressed(device, XRCommonUsages.secondary2DAxisClick))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsPressed(
        XRInputDevice device,
        InputFeatureUsage<bool> button)
    {
        bool value;

        return device.TryGetFeatureValue(button, out value) && value;
    }
}
