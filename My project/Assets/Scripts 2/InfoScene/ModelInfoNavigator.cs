
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;

// Explicit XR alias to prevent naming conflicts.
using XRInputDevice = UnityEngine.XR.InputDevice;
using XRCommonUsages = UnityEngine.XR.CommonUsages;

public class ModelInfoNavigator : MonoBehaviour
{
    [System.Serializable]
    public class ModelInformation
    {
        [Header("Model GameObject")]
        public GameObject modelObject;

        [Header("Information")]
        public string modelName;

        public string levelInfo;

        public string definitionTitle = "Definition";

        [TextArea(3, 6)]
        public string definitionText;

        public string processTitle = "Process Involved in the Simulation";

        [TextArea(3, 6)]
        public string processText;
    }

    [Header("Model List")]
    public ModelInformation[] models;

    [Header("Information UI")]
    public TMP_Text modelNameText;
    public TMP_Text levelInfoText;
    public TMP_Text definitionTitleText;
    public TMP_Text definitionText;
    public TMP_Text processTitleText;
    public TMP_Text processText;

    [Header("VR Thumbstick Settings")]
    [Range(0.1f, 1f)]
    public float thumbstickThreshold = 0.7f;

    [Range(0f, 0.5f)]
    public float thumbstickDeadZone = 0.25f;

    private int currentModelIndex = 0;

    private bool thumbstickReady = true;

    private readonly List<XRInputDevice> controllerDevices =
        new List<XRInputDevice>();

    private void Start()
    {
        if (models == null || models.Length == 0)
        {
            Debug.LogError("ModelInfoNavigator: No models assigned!");
            return;
        }

        if (modelNameText == null ||
            levelInfoText == null ||
            definitionTitleText == null ||
            definitionText == null ||
            processTitleText == null ||
            processText == null)
        {
            Debug.LogError(
                "ModelInfoNavigator: One or more UI references are missing!"
            );
            return;
        }

        // Display the first model.
        currentModelIndex = 0;
        ShowCurrentModel();

        Debug.Log("Model information viewer initialized.");
    }

    private void Update()
    {
        if (models == null || models.Length == 0)
            return;

        // Keyboard navigation.
        if (Keyboard.current != null)
        {
            if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                NextModel();
                return;
            }

            if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
            {
                PreviousModel();
                return;
            }
        }

        // VR thumbstick navigation.
        HandleThumbstickInput();
    }

    private void HandleThumbstickInput()
    {
        controllerDevices.Clear();

        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.Controller,
            controllerDevices
        );

        float strongestHorizontalInput = 0f;

        foreach (XRInputDevice device in controllerDevices)
        {
            Vector2 primaryAxis;
            Vector2 secondaryAxis;

            if (device.TryGetFeatureValue(
                XRCommonUsages.primary2DAxis,
                out primaryAxis))
            {
                if (Mathf.Abs(primaryAxis.x) >
                    Mathf.Abs(strongestHorizontalInput))
                {
                    strongestHorizontalInput = primaryAxis.x;
                }
            }

            if (device.TryGetFeatureValue(
                XRCommonUsages.secondary2DAxis,
                out secondaryAxis))
            {
                if (Mathf.Abs(secondaryAxis.x) >
                    Mathf.Abs(strongestHorizontalInput))
                {
                    strongestHorizontalInput = secondaryAxis.x;
                }
            }
        }

        // Allow another selection once the thumbstick returns to neutral.
        if (Mathf.Abs(strongestHorizontalInput) <= thumbstickDeadZone)
        {
            thumbstickReady = true;
            return;
        }

        if (!thumbstickReady)
            return;

        // Move to the next model.
        if (strongestHorizontalInput >= thumbstickThreshold)
        {
            thumbstickReady = false;
            NextModel();
        }
        // Move to the previous model.
        else if (strongestHorizontalInput <= -thumbstickThreshold)
        {
            thumbstickReady = false;
            PreviousModel();
        }
    }

    public void NextModel()
    {
        if (models == null || models.Length <= 1)
            return;

        currentModelIndex++;

        // Loop back to the first model.
        if (currentModelIndex >= models.Length)
        {
            currentModelIndex = 0;
        }

        ShowCurrentModel();
    }

    public void PreviousModel()
    {
        if (models == null || models.Length <= 1)
            return;

        currentModelIndex--;

        // Loop back to the last model.
        if (currentModelIndex < 0)
        {
            currentModelIndex = models.Length - 1;
        }

        ShowCurrentModel();
    }

    private void ShowCurrentModel()
    {
        // Disable all models first.
        foreach (ModelInformation model in models)
        {
            if (model != null && model.modelObject != null)
            {
                model.modelObject.SetActive(false);
            }
        }

        ModelInformation currentModel = models[currentModelIndex];

        if (currentModel == null)
            return;

        // Enable only the selected model.
        if (currentModel.modelObject != null)
        {
            currentModel.modelObject.SetActive(true);

            Debug.Log("Enabled model: " + currentModel.modelName);
        }
        else
        {
            Debug.LogWarning(
                "Model GameObject is missing at index " + currentModelIndex
            );
        }

        // Update the information panel.
        modelNameText.text = currentModel.modelName;

        levelInfoText.text = currentModel.levelInfo;

        definitionTitleText.text = currentModel.definitionTitle;
        definitionText.text = currentModel.definitionText;

        processTitleText.text = currentModel.processTitle;
        processText.text = currentModel.processText;

        Debug.Log(
            "Currently displaying model: " + currentModel.modelName
        );
    }
}
