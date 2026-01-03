using UnityEngine;
using UnityEngine.InputSystem; // Required for the New Input System

public class FlyCamera : MonoBehaviour
{
    [Header("Movement Settings")]
    public float rotationSensitivity = 0.5f; // Sensitivity values differ in New Input System
    public float scrollSensitivity = 0.5f; // Sensitivity values differ in New Input System
    public float movementSensitivity = 0.5f; // Sensitivity values differ in New Input System
    
    public const float MIN_SPEED = 0.01f;
    public const float MAX_SPEED = 10f;
    public float currentSpeed = 0; 

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotationX = rot.y;
        rotationY = -rot.x;
        currentSpeed = (MIN_SPEED + MAX_SPEED) / 2f;
    }

    void Update()
    {
        // 1. Speed Control (Mouse Scroll)
        if (Mouse.current != null)
        {
            float scroll = Mouse.current.scroll.ReadValue().y;
            if (scroll != 0)
            {
                // Scroll values in new system are usually ~120 per notch
                currentSpeed += (scroll / 120f) * scrollSensitivity; 
                currentSpeed = Mathf.Clamp(currentSpeed, MIN_SPEED, MAX_SPEED);
            }
        }

        // 2. Rotation Logic (Right-click to look)
        if (Mouse.current != null && Mouse.current.rightButton.isPressed)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            rotationX += mouseDelta.x * rotationSensitivity;
            rotationY += mouseDelta.y * rotationSensitivity;
            rotationY = Mathf.Clamp(rotationY, -90, 90);

            transform.localRotation = Quaternion.Euler(-rotationY, rotationX, 0);
        }

        // 3. Translation Logic (WASD + QE)
        if (Keyboard.current != null)
        {
            Vector3 direction = Vector3.zero;

            // WASD
            if (Keyboard.current.wKey.isPressed) direction += transform.forward;
            if (Keyboard.current.sKey.isPressed) direction -= transform.forward;
            if (Keyboard.current.aKey.isPressed) direction -= transform.right;
            if (Keyboard.current.dKey.isPressed) direction += transform.right;

            // QE (Up/Down)
            if (Keyboard.current.eKey.isPressed) direction += transform.up;
            if (Keyboard.current.qKey.isPressed) direction -= transform.up;

            // Apply movement scaled by your requested speed
            // Multiplier 1000f used to make 0.01-10 usable at Earth scale
            transform.position += direction * (currentSpeed * 1000f * Time.deltaTime * movementSensitivity);
        }
    }
}