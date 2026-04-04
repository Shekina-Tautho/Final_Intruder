using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement; // Namespace of ContinuousMoveProvider

public class BoostVR : MonoBehaviour
{
    [Header("Input")]
    public InputActionProperty boostAction; // Assign Right Grip here

    [Header("Speed Settings")]
    public float normalSpeed = 2f;
    public float boostSpeed = 5f;
    public float smoothTime = 5f;

    private float currentSpeed;
    private ContinuousMoveProvider moveProvider;

    void Awake()
    {
        // Find the ContinuousMoveProvider in the scene
        moveProvider = FindObjectOfType<ContinuousMoveProvider>();
        if (moveProvider == null)
        {
            Debug.LogError("No ContinuousMoveProvider found in scene!");
        }
    }

    void Start()
    {
        currentSpeed = normalSpeed;
    }

    void Update()
    {
        if (moveProvider == null) return;

        float boostValue = boostAction.action.ReadValue<float>();
        float targetSpeed = boostValue > 0.1f ? boostSpeed : normalSpeed;

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * smoothTime);
        moveProvider.moveSpeed = currentSpeed;
    }
}