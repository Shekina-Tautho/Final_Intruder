using UnityEngine;
using UnityEngine.InputSystem;

public class VerticalMovementVR : MonoBehaviour
{
    public float verticalSpeed = 2f;

    public InputActionProperty moveUpAction;    // Left Trigger
    public InputActionProperty moveDownAction;  // Left Grip

    void Update()
    {
        float upValue = moveUpAction.action.ReadValue<float>();
        float downValue = moveDownAction.action.ReadValue<float>();

        float vertical = upValue - downValue;

        Vector3 move = new Vector3(0, vertical * verticalSpeed * Time.deltaTime, 0);

        transform.position += move;
    }
}