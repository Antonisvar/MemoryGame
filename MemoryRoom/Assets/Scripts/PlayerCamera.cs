using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    //Variables
    public Transform player;
    public float mouseSensitivity = 200f;
    float cameraVerticalRotation = 0f;
    bool lockedCursor = true;
    // Start is called before the first frame update
    void Start()
    {
        //Lock the cursor
        if (lockedCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Collect mouse input
        float inputX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float inputY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        //Rotate the Camera around its local X axis

        cameraVerticalRotation -= inputY;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);
        Camera.main.transform.localRotation = Quaternion.Euler(cameraVerticalRotation, 0f, 0f);

        //Rotate the Player around its Y axis
        player.Rotate(Vector3.up * inputX);




    }
}
