using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCameraX : MonoBehaviour
{

    private float xRotation;
    private float yRotation;

    public Transform orientation;

    public float sensitivity = 2.0f; // Mouse sensitivity
    public float minYAngle = -60.0f; // Minimum Y angle
    public float maxYAngle = 60.0f; // Maximum Y angle

    private Transform cameraTransform;

    // Start is called before the first frame update
    void Start()
    {
        // Lock the cursor to the center of the game window
        Cursor.lockState = CursorLockMode.Locked;
        // Hide the cursor
        Cursor.visible = false;

        cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        // Get mouse movement
        float mouseX = Input.GetAxisRaw("Mouse X") * sensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensitivity;

        // Add mouse movement to current rotation
        yRotation += mouseX;

        xRotation -= mouseY;

        // Prevent the player from flipping the camera
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotate the camera based on mouse movement
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);

        
    }
}
