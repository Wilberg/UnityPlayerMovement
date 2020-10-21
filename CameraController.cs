using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public PlayerController player;

    public float sensitivity = 3.0f;
    private float sensitivityMultiplier = 200.0f;

    private float mouseX = 0f;
    private float mouseY = 0f;
    private float rotationX;
    private float rotationY;

    // Update is called once per frame
    void Update()
    {
        transform.position = player.head.position;
        Look();
    }

    void Look()
    {
        float mouseX = PlayerInput.mouseX * sensitivity * sensitivityMultiplier * Time.deltaTime;
        float mouseY = PlayerInput.mouseY * sensitivity * sensitivityMultiplier * Time.deltaTime;

        Vector3 cameraRotation = transform.localRotation.eulerAngles;
        rotationY = cameraRotation.y + mouseX;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -60.0f, 60.0f);

        transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0);
        player.transform.localRotation = Quaternion.Euler(0, rotationY, 0);
    }
}
