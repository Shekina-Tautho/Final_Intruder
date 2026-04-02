#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLookVRTest : MonoBehaviour
{
    public float sensitivity = 2f;

    private PlayerInputActions controls;
    private Vector2 lookInput;

    private float xRotation = 0f;

    void Awake()
    {
        controls = new PlayerInputActions();

        controls.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Player.Look.canceled += ctx => lookInput = Vector2.zero;
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Update()
    {
        float mouseX = lookInput.x * sensitivity;
        float mouseY = lookInput.y * sensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        if (transform.parent != null)
            transform.parent.Rotate(Vector3.up * mouseX);
    }
}
#endif