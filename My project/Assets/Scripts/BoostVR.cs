using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;

public class BoostVR : MonoBehaviour
{
    [Header("References")]
    public PlayerStats playerStats;

    [Header("Input")]
    public InputActionProperty boostAction;

    [Header("Speed Settings")]
    public float normalSpeed = 2f;
    public float boostSpeed = 5f;
    public float smoothTime = 5f;

    private float currentSpeed;
    private ContinuousMoveProvider moveProvider;

    void Awake()
    {
        moveProvider = FindObjectOfType<ContinuousMoveProvider>();
    }

    void Start()
    {
        currentSpeed = normalSpeed;
    }

    void Update()
    {
        if (moveProvider == null || playerStats == null) return;

        float boostValue = boostAction.action.ReadValue<float>();

        bool isBoosting = boostValue > 0.1f && playerStats.HasStamina();

        if (isBoosting)
        {
            playerStats.UseStamina(playerStats.staminaDrainRate * Time.deltaTime);
        }

        float targetSpeed = isBoosting ? boostSpeed : normalSpeed;

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * smoothTime);
        moveProvider.moveSpeed = currentSpeed;

        Debug.Log("Stamina: " + playerStats.currentStamina);
        Debug.Log("Has stamina? " + playerStats.HasStamina());
    }
}