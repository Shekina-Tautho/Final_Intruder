
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;

// Explicit XR aliases to prevent naming conflicts.
using XRInputDevice = UnityEngine.XR.InputDevice;
using XRCommonUsages = UnityEngine.XR.CommonUsages;

public class TutorialManager : MonoBehaviour
{
    [System.Serializable]
    public class TutorialStep
    {
        public string title;

        [TextArea(2, 5)]
        public string instruction;

        [Header("Controller Model")]
        public GameObject controllerModel;

        [Header("Controller Rotation")]
        public Transform rotationTarget;

        public Vector3 controllerRotation = Vector3.zero;

        [Header("Highlighted Parts")]
        public GameObject[] highlights;
    }

    [Header("Tutorial Settings")]
    public bool skipTutorial = false;

    [Header("Tutorial UI")]
    public GameObject tutorialPanel;
    public TMP_Text titleText;
    public TMP_Text instructionText;
    public TMP_Text pressAnyButtonText;

    [Header("Tutorial Audio")]
    public AudioSource audioSource;
    public AudioClip buttonClickSound;

    [Range(0f, 1f)]
    public float buttonClickVolume = 1f;

    [Header("Existing Simulation UI")]
    public GameObject simulationUI;

    [Header("Gameplay")]
    public VirusSpawner virusSpawner;

    [Tooltip("Enable this only in Scene 3.")]
    public bool useVirusReleaseManager = false;

    public VirusReleaseManager virusReleaseManager;

    [Header("Player Visibility")]
    public GameObject[] playerVisuals;

    [Header("Tutorial Steps")]
    public TutorialStep[] steps;

    private int currentStep = 0;
    private bool previousButtonState = false;
    private bool tutorialStarted = false;

    private readonly List<XRInputDevice> controllerDevices =
        new List<XRInputDevice>();

    private void Awake()
    {
        if (simulationUI != null)
        {
            simulationUI.SetActive(false);
            Debug.Log("SimulationUI hidden.");
        }
        else
        {
            Debug.LogError("SimulationUI is not assigned!");
        }
    }

    private void Start()
    {
        if (skipTutorial)
        {
            Debug.Log("Tutorial skipped.");

            if (tutorialPanel != null)
                tutorialPanel.SetActive(false);

            if (pressAnyButtonText != null)
                pressAnyButtonText.gameObject.SetActive(false);

            SetPlayerVisibility(true);

            if (simulationUI != null)
                simulationUI.SetActive(true);

            StartGameplay();
            return;
        }

        if (tutorialPanel == null ||
            titleText == null ||
            instructionText == null ||
            pressAnyButtonText == null ||
            simulationUI == null)
        {
            Debug.LogError(
                "TutorialManager: Required tutorial references are missing!"
            );
            return;
        }

        if (useVirusReleaseManager && virusReleaseManager == null)
        {
            Debug.LogError(
                "TutorialManager: VirusReleaseManager is missing!"
            );
            return;
        }

        if (!useVirusReleaseManager && virusSpawner == null)
        {
            Debug.LogError(
                "TutorialManager: VirusSpawner is missing!"
            );
            return;
        }

        if (steps == null || steps.Length == 0)
        {
            Debug.LogError("No tutorial steps configured!");
            return;
        }

        for (int i = 0; i < steps.Length; i++)
        {
            if (steps[i] == null)
            {
                Debug.LogError("Tutorial step " + i + " is empty!");
                return;
            }
        }

        SetPlayerVisibility(false);

        simulationUI.SetActive(false);

        tutorialPanel.SetActive(true);
        pressAnyButtonText.gameObject.SetActive(true);

        previousButtonState = IsAnyControllerButtonPressed();

        tutorialStarted = true;

        ShowStep();

        Debug.Log("Tutorial started successfully.");
    }

    private void Update()
    {
        if (!tutorialStarted)
            return;

        if (tutorialPanel == null ||
            !tutorialPanel.activeInHierarchy)
            return;

        // Check VR controller buttons.
        bool controllerPressed = IsAnyControllerButtonPressed();

        // Detect a new controller button press.
        bool controllerJustPressed =
            controllerPressed && !previousButtonState;

        // Check keyboard Enter and Numpad Enter.
        bool keyboardPressed = false;

        if (Keyboard.current != null)
        {
            keyboardPressed =
                Keyboard.current.enterKey.wasPressedThisFrame ||
                Keyboard.current.numpadEnterKey.wasPressedThisFrame;
        }

        // Advance tutorial using either input method.
        if (controllerJustPressed || keyboardPressed)
        {
            if (audioSource != null && buttonClickSound != null)
            {
                audioSource.PlayOneShot(
                    buttonClickSound,
                    buttonClickVolume
                );
            }

            NextStep();
        }

        previousButtonState = controllerPressed;
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

    public void NextStep()
    {
        if (!tutorialStarted)
            return;

        currentStep++;

        if (currentStep >= steps.Length)
        {
            FinishTutorial();
            return;
        }

        ShowStep();
    }

    private void ShowStep()
    {
        if (currentStep < 0 || currentStep >= steps.Length)
            return;

        TutorialStep step = steps[currentStep];

        titleText.text = step.title;
        instructionText.text = step.instruction;

        foreach (TutorialStep tutorialStep in steps)
        {
            if (tutorialStep.controllerModel != null)
                tutorialStep.controllerModel.SetActive(false);

            if (tutorialStep.highlights != null)
            {
                foreach (GameObject highlight in tutorialStep.highlights)
                {
                    if (highlight != null)
                        highlight.SetActive(false);
                }
            }
        }

        if (step.controllerModel != null)
            step.controllerModel.SetActive(true);

        if (step.rotationTarget != null)
        {
            step.rotationTarget.localRotation =
                Quaternion.Euler(step.controllerRotation);
        }
        else
        {
            Debug.LogWarning(
                "No Rotation Target assigned for tutorial step: " +
                step.title
            );
        }

        if (step.highlights != null)
        {
            foreach (GameObject highlight in step.highlights)
            {
                if (highlight != null)
                    highlight.SetActive(true);
            }
        }

        Debug.Log(
            "Tutorial Step " + (currentStep + 1) +
            ": " + step.title
        );
    }

    private void SetPlayerVisibility(bool isVisible)
    {
        if (playerVisuals == null || playerVisuals.Length == 0)
        {
            Debug.LogWarning("Player Visuals array is empty.");
            return;
        }

        foreach (GameObject visual in playerVisuals)
        {
            if (visual != null)
                visual.SetActive(isVisible);
        }
    }

    private void FinishTutorial()
    {
        tutorialStarted = false;

        tutorialPanel.SetActive(false);
        pressAnyButtonText.gameObject.SetActive(false);

        foreach (TutorialStep step in steps)
        {
            if (step.controllerModel != null)
                step.controllerModel.SetActive(false);

            if (step.highlights != null)
            {
                foreach (GameObject highlight in step.highlights)
                {
                    if (highlight != null)
                        highlight.SetActive(false);
                }
            }
        }

        SetPlayerVisibility(true);

        simulationUI.SetActive(true);

        Debug.Log("Tutorial completed.");

        StartGameplay();
    }

    private void StartGameplay()
    {
        if (useVirusReleaseManager)
        {
            if (virusReleaseManager == null)
            {
                Debug.LogError(
                    "VirusReleaseManager is not assigned!"
                );
                return;
            }

            virusReleaseManager.BeginRelease();

            Debug.Log("Scene 3 virus release started.");
        }
        else
        {
            if (virusSpawner == null)
            {
                Debug.LogError(
                    "VirusSpawner is not assigned!"
                );
                return;
            }

            virusSpawner.StartSpawning();

            Debug.Log("Normal virus simulation started.");
        }
    }
}
