using System.ComponentModel;
using UnityEngine;
using UnityEditor;

public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody rb;

    [Header("Assignments")]
    public Transform head;
    public LayerMask groundLayer;

    [Header("Physics Settings")]
    public float speed = 10.0f;
    public float acceleration = 20.0f;
    public float mass = 65.0f;
    public float brakeMultiplier = 2.0f;
    public float sprintSpeed = 20.0f;

    // Stored values for general use.
    private float maxSpeed;
    private Vector3 playerScale;
    private bool isGrounded;

    // Multipliers.
    private float movementMultiplier = 1.0f;
    private float movementMultiplierVertical = 1.0f;
    private float gravityMultiplier = 2.0f;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerScale = transform.localScale;
    }

    private void Update()
    {
        PlayerInput.Update();
        rb.mass = mass;

        if (PlayerInput.isCrouching) Crouch();
        else Uncrouch();

        if (PlayerInput.isSprinting && isGrounded && !PlayerInput.isCrouching) Sprint();
        else if (isGrounded) Walk();


        if (PlayerInput.isJumping && !PlayerInput.isCrouching) Jump();
    }

    private void FixedUpdate()
    {
        rb.AddForce(0, -9.81f * mass * gravityMultiplier, 0);

        Vector2 magnitude = GetRelativeMagnitude();

        HandleMaxSpeed(magnitude);

        if (!isGrounded)
        {
            movementMultiplier = 0.2f;
            movementMultiplierVertical = 0.5f;
        } else
        {
            AddCounterForce(magnitude);
            movementMultiplier = 1.0f;
            movementMultiplierVertical = 1.0f;
        }

        rb.AddForce(transform.forward * mass * acceleration * PlayerInput.y * movementMultiplier);
        rb.AddForce(transform.right * mass * acceleration * PlayerInput.x * movementMultiplierVertical);
    }


    private void Jump()
    {
        if (isGrounded)
        {
            isGrounded = false;
            rb.AddForce(0, 5.0f * mass, 0, ForceMode.Impulse);
        }
    }

    private void Crouch()
    {
        transform.localScale = new Vector3(playerScale.x, playerScale.y / 2, playerScale.z);
    }
    private void Uncrouch()
    {
        transform.localScale = new Vector3(playerScale.x, playerScale.y, playerScale.z);
    }

    private void Sprint()
    {
        maxSpeed = sprintSpeed;
    }
    private void Walk()
    {
        maxSpeed = speed;
    }


    private void AddCounterForce(Vector2 mag)
    {
        float yAbs = Mathf.Abs(PlayerInput.y);
        float xAbs = Mathf.Abs(PlayerInput.x);

        // Add counter force when no longer moving or started strafing Y axis.
        if (yAbs < 0.05f || PlayerInput.y < 0 && mag.y > 0 || PlayerInput.y > 0 && mag.y < 0)
        {
            rb.AddForce(transform.forward * mass * -mag.y * brakeMultiplier);
        }
        // Add counter force when no longer moving or started strafing X axis.
        if (xAbs < 0.05f || PlayerInput.x < 0 && mag.x > 0 || PlayerInput.x > 0 && mag.x < 0)
        {
            rb.AddForce(transform.right * mass * -mag.x * brakeMultiplier);
        }
        // Cancel out velocity if magnitude is low.
        if (mag.sqrMagnitude <= 0.5f && mag.sqrMagnitude > 0 && xAbs < 0.05f && yAbs < 0.05f)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }

    private void HandleMaxSpeed(Vector2 mag)
    {
        if (Mathf.Abs(mag.y) > maxSpeed)
        {
            float direction = mag.y / Mathf.Abs(mag.y);
            float difference = mag.y - (maxSpeed * direction);
            rb.AddForce(transform.forward * mass * -difference, ForceMode.Impulse);
        }
        if (Mathf.Abs(mag.x) > maxSpeed)
        {
            float direction = mag.x / Mathf.Abs(mag.x);
            float difference = mag.x - (maxSpeed * direction);
            rb.AddForce(transform.right * mass * -difference, ForceMode.Impulse);
        }
    }

    private Vector2 GetRelativeMagnitude()
    {
        Vector3 magnitude = transform.InverseTransformDirection(rb.velocity);
        return new Vector2(magnitude.x, magnitude.z);
    }

    private void OnCollisionStay(Collision collision)
    {
        int layer = collision.gameObject.layer;
        if (groundLayer != (groundLayer | (1 << layer))) return;

        foreach (ContactPoint contact in collision.contacts)
        {
            Vector3 normal = contact.normal;
            if (Vector3.Angle(Vector3.up, normal) == 0.0f)
            {
                isGrounded = true;
            }
        }
    }

}
