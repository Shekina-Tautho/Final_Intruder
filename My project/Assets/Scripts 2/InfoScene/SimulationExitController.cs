
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using System.Collections.Generic;

using XRInputDevice = UnityEngine.XR.InputDevice;
using XRCommonUsages = UnityEngine.XR.CommonUsages;

public class SimulationExitController : MonoBehaviour
{
    private readonly List<XRInputDevice> rightControllerDevices =
        new List<XRInputDevice>();

    private void Update()
    {
        // Keyboard Escape
        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ExitSimulation();
        }

        // Right VR controller A button
        CheckVRExitButton();
    }

    private void CheckVRExitButton()
    {
        rightControllerDevices.Clear();

        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.Right |
            InputDeviceCharacteristics.Controller,
            rightControllerDevices
        );

        foreach (XRInputDevice device in rightControllerDevices)
        {
            bool aButtonPressed;

            if (device.TryGetFeatureValue(
                XRCommonUsages.primaryButton,
                out aButtonPressed) && aButtonPressed)
            {
                ExitSimulation();
                return;
            }
        }
    }

    public void ExitSimulation()
    {
        Debug.Log("Exiting Pathogenesis simulation.");

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
