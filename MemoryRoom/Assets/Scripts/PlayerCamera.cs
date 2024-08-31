using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform player;
    public float mouseSensitivity = 200f;
    float cameraVerticalRotation = 0f;
    bool lockedCursor = true;
    private bool canLook = true; // Variable to control if the camera can rotate

    void Start()
    {
        if (lockedCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        if (!canLook) return; // Prevent camera movement if canLook is false

        float inputX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float inputY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        cameraVerticalRotation -= inputY;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);
        Camera.main.transform.localRotation = Quaternion.Euler(cameraVerticalRotation, 0f, 0f);

        player.Rotate(Vector3.up * inputX);
    }

    // Method to enable or disable camera control
    public void SetCameraControl(bool enableLook)
    {
        canLook = enableLook;
        if (enableLook)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
