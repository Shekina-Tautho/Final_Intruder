
using UnityEngine;
using UnityEngine.XR;
using TMPro;
using System.Collections.Generic;

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
        public Vector3 controllerRotation = Vector3.zero;

        [Header("Highlighted Parts")]
        public GameObject[] highlights;
    }

    [Header("Tutorial UI")]
    public GameObject tutorialPanel;
    public TMP_Text titleText;
    public TMP_Text instructionText;
    public TMP_Text pressAnyButtonText;

    [Header("Existing Simulation UI")]
    public GameObject simulationUI;

    [Header("Gameplay")]
    public VirusSpawner virusSpawner;

    [Header("Player Visibility")]
    [Tooltip("Add the player's visible body object here. Do not add Main Camera or XR Origin.")]
    public GameObject[] playerVisuals;

    [Header("Tutorial Steps")]
    public TutorialStep[] steps;

    private int currentStep = 0;
    private bool previousButtonState = false;
    private bool tutorialStarted = false;

    private readonly List<InputDevice> controllerDevices =
        new List<InputDevice>();

    private void Awake()
    {
        // Hide gameplay UI immediately.
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
        // Validate required references.
        if (tutorialPanel == null ||
            titleText == null ||
            instructionText == null ||
            pressAnyButtonText == null ||
            simulationUI == null ||
            virusSpawner == null)
        {
            Debug.LogError(
                "TutorialManager: One or more required references are missing!"
            );
            return;
        }

        if (steps == null || steps.Length == 0)
        {
            Debug.LogError("No tutorial steps configured!");
            return;
        }

        // Validate tutorial steps.
        for (int i = 0; i < steps.Length; i++)
        {
            if (steps[i] == null)
            {
                Debug.LogError("Tutorial step " + i + " is empty!");
                return;
            }
        }

        // Hide the player's visible body.
        SetPlayerVisibility(false);

        // Ensure gameplay UI is hidden.
        simulationUI.SetActive(false);

        // Show tutorial.
        tutorialPanel.SetActive(true);
        pressAnyButtonText.gameObject.SetActive(true);

        // Prevent a button already held at startup
        // from immediately skipping the first step.
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

        bool buttonPressed = IsAnyControllerButtonPressed();

        // Detect a fresh button press.
        if (buttonPressed && !previousButtonState)
        {
            NextStep();
        }

        previousButtonState = buttonPressed;
    }

    private bool IsAnyControllerButtonPressed()
    {
        controllerDevices.Clear();

        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.Controller,
            controllerDevices
        );

        foreach (InputDevice device in controllerDevices)
        {
            if (IsPressed(device, CommonUsages.primaryButton) ||
                IsPressed(device, CommonUsages.secondaryButton) ||
                IsPressed(device, CommonUsages.triggerButton) ||
                IsPressed(device, CommonUsages.gripButton) ||
                IsPressed(device, CommonUsages.menuButton) ||
                IsPressed(device, CommonUsages.primary2DAxisClick) ||
                IsPressed(device, CommonUsages.secondary2DAxisClick))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsPressed(
        InputDevice device,
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

        // Update instructional text.
        titleText.text = step.title;
        instructionText.text = step.instruction;

        // Hide all controller models and highlights.
        foreach (TutorialStep tutorialStep in steps)
        {
            if (tutorialStep.controllerModel != null)
            {
                tutorialStep.controllerModel.SetActive(false);
            }

            if (tutorialStep.highlights != null)
            {
                foreach (GameObject highlight in tutorialStep.highlights)
                {
                    if (highlight != null)
                    {
                        highlight.SetActive(false);
                    }
                }
            }
        }

        // Show only the controller for this step.
        if (step.controllerModel != null)
        {
            step.controllerModel.SetActive(true);

            // Apply this step's controller rotation.
            step.controllerModel.transform.localEulerAngles =
                step.controllerRotation;
        }

        // Show only the current step's highlights.
        if (step.highlights != null)
        {
            foreach (GameObject highlight in step.highlights)
            {
                if (highlight != null)
                {
                    highlight.SetActive(true);
                }
            }
        }

        Debug.Log(
            "Tutorial Step " + (currentStep + 1) +
            ": " + step.title +
            " | Controller Rotation: " + step.controllerRotation
        );
    }

    private void SetPlayerVisibility(bool isVisible)
    {
        if (playerVisuals == null || playerVisuals.Length == 0)
        {
            Debug.LogWarning(
                "Player Visuals array is empty. No player objects will be changed."
            );
            return;
        }

        foreach (GameObject visual in playerVisuals)
        {
            if (visual != null)
            {
                visual.SetActive(isVisible);

                Debug.Log(
                    "Player object: " + visual.name +
                    " | Visibility: " + isVisible
                );
            }
            else
            {
                Debug.LogWarning(
                    "Player Visuals contains an empty reference!"
                );
            }
        }
    }

    private void FinishTutorial()
    {
        tutorialStarted = false;

        // Hide tutorial interface.
        tutorialPanel.SetActive(false);

        // Hide instruction prompt.
        pressAnyButtonText.gameObject.SetActive(false);

        // Hide all tutorial controller models and highlights.
        foreach (TutorialStep step in steps)
        {
            if (step.controllerModel != null)
            {
                step.controllerModel.SetActive(false);
            }

            if (step.highlights != null)
            {
                foreach (GameObject highlight in step.highlights)
                {
                    if (highlight != null)
                    {
                        highlight.SetActive(false);
                    }
                }
            }
        }

        // Restore the player's visible body.
        SetPlayerVisibility(true);

        // Restore gameplay UI.
        simulationUI.SetActive(true);

        Debug.Log("SimulationUI restored.");

        // Start virus simulation.
        virusSpawner.StartSpawning();

        Debug.Log("Tutorial completed. Virus simulation started!");
    }
}
