using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float maxSpeed = 5f; // Maximum speed the player can reach
    public float acceleration = 50f; // How quickly the player accelerates
    public float deceleration = 30f; // How quickly the player slows down
    public float collisionOffset = 0.05f;
    public float rotateCooldown = 0.5f;
    public float rotationSpeed = 5f;
    public ContactFilter2D movementFilter;

    private Vector2 movementInput;
    private Vector2 currentVelocity;
    private Rigidbody2D rb;
    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    private bool canRotate;
    private bool isRotating = false;
    private float targetAngle;

    // Camera bounds variables
    private Camera mainCamera;
    private Vector2 minBounds;
    private Vector2 maxBounds;
    private Vector2 playerExtents; // Half the size of the player's collider

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        canRotate = true;
        isRotating = false;

        // Initialize camera and bounds
        mainCamera = Camera.main;
        CalculateCameraBounds();
    }

    // Calculate the camera bounds and player extents
    private void CalculateCameraBounds()
    {
        if (mainCamera == null) return;

        // Get the world coordinates of the camera's viewport
        minBounds = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        maxBounds = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, 0));

        // Get the player's collider bounds to account for its size
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            playerExtents = collider.bounds.extents; // Half the width and height
        }
        else
        {
            playerExtents = Vector2.zero; // Fallback if no collider
        }

        // Adjust bounds to prevent the player's edges from crossing the camera borders
        minBounds += playerExtents;
        maxBounds -= playerExtents;
    }

    // Update is called once per frame
    void Update()
    {
        // Get input every frame
        // Note: OnMove handles input updates

        if (isRotating)
        {
            float currentAngle = transform.eulerAngles.z;
            float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, 0, newAngle);

            if (Mathf.Approximately(newAngle, targetAngle))
            {
                transform.rotation = Quaternion.Euler(0, 0, targetAngle);
                isRotating = false;
                StartCoroutine(ResetRotateCooldown());
            }
        }
    }

    private void FixedUpdate()
    {
        // Calculate target velocity based on input
        Vector2 targetVelocity = movementInput * maxSpeed;

        // Smoothly interpolate current velocity towards target velocity
        if (movementInput != Vector2.zero)
        {
            // Accelerate towards target velocity
            currentVelocity = Vector2.MoveTowards(
                currentVelocity,
                targetVelocity,
                acceleration * Time.fixedDeltaTime
            );
        }
        else
        {
            // Decelerate to zero when no input
            currentVelocity = Vector2.MoveTowards(
                currentVelocity,
                Vector2.zero,
                deceleration * Time.fixedDeltaTime
            );
        }

        // Check for collisions using raycast
        int count = rb.Cast(
            currentVelocity.normalized,
            movementFilter,
            castCollisions,
            currentVelocity.magnitude * Time.fixedDeltaTime + collisionOffset
        );

        // Calculate the new position
        Vector2 newPosition = rb.position + currentVelocity * Time.fixedDeltaTime;

        // Move if no collisions detected
        if (count == 0)
        {
            // Clamp the new position to stay within camera bounds
            // newPosition.x = Mathf.Clamp(newPosition.x, minBounds.x, maxBounds.x);
            // newPosition.y = Mathf.Clamp(newPosition.y, minBounds.y, maxBounds.y);

            // Apply the clamped position
            rb.MovePosition(newPosition);
        }
        else
        {
            // Stop velocity in direction of collision
            currentVelocity = Vector2.zero;
        }
    }

    void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }

    void OnRotateLeft()
    {
        if (canRotate && !isRotating)
        {   
            targetAngle = transform.eulerAngles.z + 90;
            isRotating = true;
            canRotate = false;
            // transform.Rotate(0, 0, 90);
            // StartCoroutine(RotateOverTime(90));
            // canRotate = false;

            // StartCoroutine(ResetRotateCooldown());
        }
    }

    void OnRotateRight()
    {
        if (canRotate && !isRotating)
        {
            // transform.Rotate(0, 0, - 90);
            // StartCoroutine(RotateOverTime(-90));
            // canRotate = false;

            // StartCoroutine(ResetRotateCooldown());
            targetAngle = transform.eulerAngles.z - 90;
            isRotating = true;
            canRotate = false;
        }
    }

    IEnumerator ResetRotateCooldown()
    {
        yield return new WaitForSeconds(rotateCooldown);
        canRotate = true;
    }

    // Optional: Recalculate bounds if the camera or screen size changes
    void OnEnable()
    {
        // Recalculate bounds when the object is enabled
        CalculateCameraBounds();
    }
}