
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

    [Header("Tutorial Steps")]
    public TutorialStep[] steps;

    private int currentStep = 0;
    private bool previousButtonState = false;
    private bool tutorialStarted = false;

    private readonly List<InputDevice> controllerDevices =
        new List<InputDevice>();

    private void Awake()
    {
        // Hide gameplay UI immediately
        if (simulationUI != null)
        {
            simulationUI.SetActive(false);
            Debug.Log("SimulationUI hidden.");
        }
        else
        {
            Debug.LogError(
                "SimulationUI is not assigned in TutorialManager!"
            );
        }
    }

    private void Start()
    {
        // Validate required references
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

        // Ensure gameplay UI is hidden
        simulationUI.SetActive(false);

        // Show tutorial
        tutorialPanel.SetActive(true);
        pressAnyButtonText.gameObject.SetActive(true);

        // Prevent a button already held at startup
        // from immediately skipping the first step
        previousButtonState = IsAnyControllerButtonPressed();

        tutorialStarted = true;

        ShowStep();

        Debug.Log("Tutorial started.");
    }

    private void Update()
    {
        if (!tutorialStarted)
            return;

        if (tutorialPanel == null ||
            !tutorialPanel.activeInHierarchy)
            return;

        bool buttonPressed = IsAnyControllerButtonPressed();

        // Detect a fresh button press
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
        TutorialStep step = steps[currentStep];

        // Update instructional text
        titleText.text = step.title;
        instructionText.text = step.instruction;

        // Hide all controller models and highlights
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

        // Show current controller model
        if (step.controllerModel != null)
        {
            step.controllerModel.SetActive(true);
        }

        // Highlight current controller parts
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
            ": " + step.title
        );
    }

    private void FinishTutorial()
    {
        tutorialStarted = false;

        // Hide tutorial interface
        tutorialPanel.SetActive(false);

        // Hide instruction prompt
        pressAnyButtonText.gameObject.SetActive(false);

        // Restore gameplay UI
        simulationUI.SetActive(true);

        Debug.Log("SimulationUI restored.");

        // Start virus simulation
        virusSpawner.StartSpawning();

        Debug.Log("Tutorial completed. Virus simulation started!");
    }
}
