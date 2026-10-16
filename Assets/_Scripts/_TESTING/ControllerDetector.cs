using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerDetector : MonoBehaviour
{
    void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;

        // Initial check for currently connected gamepads
        if (Gamepad.current != null)
        {
            Debug.Log($"Controller already connected: {Gamepad.current.displayName}");
        }
    }
    void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        // Check if the affected device is a gamepad
        if (device is Gamepad)
        {
            switch (change)
            {
                case InputDeviceChange.Added:
                    Debug.Log($"Controller Connected: {device.displayName}");
                    break;
                case InputDeviceChange.Removed:
                    Debug.Log($"Controller Disconnected: {device.displayName}");
                    break;
            }
        }
    }
}
