using UnityEngine;

public class PlayerInput
{

    // Variables to contain values for player input.
    public static float x;
    public static float y;
    public static float mouseX;
    public static float mouseY;
    public static bool isJumping;
    public static bool isCrouching;
    public static bool isSprinting;

    public static void Update()
    {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
        isJumping = Input.GetButton("Jump");
        isCrouching = Input.GetKey(KeyCode.LeftControl);
        isSprinting = Input.GetKey(KeyCode.LeftShift);
    }
}
