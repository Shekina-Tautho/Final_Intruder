using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class XRFallbackLocomotion : MonoBehaviour
{
    [Header("References")]
    public Transform xrCamera;
    public CharacterController characterController;

    [Header("Input Actions")]
    public InputActionProperty moveAction;
    public InputActionProperty lookAction;
    public InputActionProperty verticalAction;
    public InputActionProperty boostAction;

    [Header("Movement Settings")]
    public float moveSpeed = 2.5f;
    public float verticalSpeed = 2f;
    public float lookSpeed = 2f;
    public float boostMultiplier = 2f;

    private float currentSpeed;

    void Start()
    {
        currentSpeed = moveSpeed;
    }

    void OnEnable()
    {
        moveAction.action.Enable();
        lookAction.action.Enable();
        verticalAction.action.Enable();
        boostAction.action.Enable();
    }

    void OnDisable()
    {
        moveAction.action.Disable();
        lookAction.action.Disable();
        verticalAction.action.Disable();
        boostAction.action.Disable();
    }

    void Update()
    {
        // Only run fallback if VR is NOT active
        if (XRSettings.isDeviceActive)
            return;

        HandleMovement();
        HandleLook();
        HandleVertical();
        HandleBoost();
    }

    void HandleMovement()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        Vector3 forward = xrCamera.forward;
        Vector3 right = xrCamera.right;

        forward.y = 0;
        right.y = 0;

        Vector3 move = (forward * input.y + right * input.x).normalized;

        characterController.Move(move * currentSpeed * Time.deltaTime);
    }

    void HandleLook()
    {
        Vector2 look = lookAction.action.ReadValue<Vector2>();

        transform.Rotate(Vector3.up * look.x * lookSpeed * Time.deltaTime);
    }

    void HandleVertical()
    {
        float vertical = verticalAction.action.ReadValue<float>();

        Vector3 move = Vector3.up * vertical * verticalSpeed;

        characterController.Move(move * Time.deltaTime);
    }

    void HandleBoost()
    {
        float boost = boostAction.action.ReadValue<float>();

        bool isBoosting = boost > 0.5f;

        currentSpeed = isBoosting
            ? moveSpeed * boostMultiplier
            : moveSpeed;
    }
}