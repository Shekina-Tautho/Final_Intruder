
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

    [Header("Gameplay")]
    public VirusSpawner virusSpawner;

    [Header("Tutorial Steps")]
    public TutorialStep[] steps;

    private int currentStep = 0;

    private bool previousButtonState = false;

    private readonly List<InputDevice> controllerDevices =
        new List<InputDevice>();

    private void Start()
    {
        if (tutorialPanel == null)
        {
            Debug.LogError("Tutorial Panel is not assigned!");
            return;
        }

        tutorialPanel.SetActive(true);

        if (steps == null || steps.Length == 0)
        {
            Debug.LogError("No tutorial steps configured!");
            return;
        }

        ShowStep();
    }

    private void Update()
    {
        if (tutorialPanel == null || !tutorialPanel.activeInHierarchy)
            return;

        bool buttonPressed = IsAnyControllerButtonPressed();

        // Detect a new button press, not a button being held down
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
            // Common controller buttons
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

        titleText.text = step.title;
        instructionText.text = step.instruction;

        // Hide all controller models and highlights
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

        // Show the current controller
        if (step.controllerModel != null)
            step.controllerModel.SetActive(true);

        // Highlight the current control
        if (step.highlights != null)
        {
            foreach (GameObject highlight in step.highlights)
            {
                if (highlight != null)
                    highlight.SetActive(true);
            }
        }

        Debug.Log("Tutorial Step: " + step.title);
    }

    private void FinishTutorial()
    {
        tutorialPanel.SetActive(false);

        if (virusSpawner != null)
        {
            virusSpawner.StartSpawning();
        }
        else
        {
            Debug.LogError("VirusSpawner is not assigned!");
        }

        Debug.Log("Tutorial completed. Virus simulation started!");
    }
}
