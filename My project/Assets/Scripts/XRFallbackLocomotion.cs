using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class XRFallbackLocomotion : MonoBehaviour
{
    [Header("References")]
    public Transform xrCamera;
    public CharacterController characterController;

    [Header("Player Stats")]
    public PlayerStats playerStats;

    [Header("Input Actions")]
    public InputActionProperty moveAction;
    public InputActionProperty lookAction;
    public InputActionProperty verticalAction;
    public InputActionProperty boostAction;
    public InputActionProperty infectAction; // ✅ NEW

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
        infectAction.action.Enable(); // ✅ NEW
    }

    void OnDisable()
    {
        moveAction.action.Disable();
        lookAction.action.Disable();
        verticalAction.action.Disable();
        boostAction.action.Disable();
        infectAction.action.Disable(); // ✅ NEW
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
        HandleInfect(); // ✅ NEW
    }

    // ---------------- MOVEMENT ----------------
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

    // ---------------- LOOK ----------------
    void HandleLook()
    {
        Vector2 look = lookAction.action.ReadValue<Vector2>();

        transform.Rotate(Vector3.up * look.x * lookSpeed * Time.deltaTime);
    }

    // ---------------- VERTICAL ----------------
    void HandleVertical()
    {
        float vertical = verticalAction.action.ReadValue<float>();

        Vector3 move = Vector3.up * vertical * verticalSpeed;

        characterController.Move(move * Time.deltaTime);
    }

    // ---------------- BOOST ----------------
    void HandleBoost()
    {
        if (playerStats == null)
        {
            currentSpeed = moveSpeed;
            return;
        }

        float boost = boostAction.action.ReadValue<float>();

        bool isBoosting = boost > 0.5f && playerStats.HasStamina();

        if (isBoosting)
        {
            playerStats.UseStamina(playerStats.staminaDrainRate * Time.deltaTime);
        }

        float targetSpeed = isBoosting ? moveSpeed * boostMultiplier : moveSpeed;

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 5f);
    }

    // ---------------- INFECT ----------------
    void HandleInfect()
    {
        if (infectAction.action.WasPressedThisFrame())
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, 2f);

            foreach (var hit in hits)
            {
                CellInfect cell = hit.GetComponent<CellInfect>();

                if (cell != null)
                {
                    cell.TryInfect();

                    Debug.Log("Infect triggered");
                }
            }
        }
    }
}