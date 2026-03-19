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
    public float knockbackDrag = 5f;
    public ContactFilter2D movementFilter;

    private Vector2 movementInput;
    private Vector2 currentVelocity;
    private Rigidbody2D rb;
    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    private bool canRotate;
    private bool isRotating = false;
    private float targetAngle;
    private bool isKnockedBack;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        canRotate = true;
        isRotating = false;
        isKnockedBack = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Get input every frame
        // Note: OnMove handles input updates
    }

    private void FixedUpdate()
    {
        // Taking knockback
        if (isKnockedBack)
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, knockbackDrag * Time.fixedDeltaTime);

            if (rb.linearVelocity.magnitude < 0.1f)
            {
                rb.linearVelocity = Vector2.zero;
                isKnockedBack = false;
            }

            return;
        }

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
            rb.MovePosition(newPosition);
        }
        else
        {
            // Stop velocity in direction of collision
            currentVelocity = Vector2.zero;
        }

        // Rotation
        if (isRotating)
        {
            float currentAngle = transform.eulerAngles.z;
            float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
            rb.MoveRotation(newAngle);
            // transform.rotation = Quaternion.Euler(0, 0, newAngle);

            if (Mathf.Approximately(newAngle, targetAngle))
            {
                rb.rotation = targetAngle;
                // transform.rotation = Quaternion.Euler(0, 0, targetAngle);
                isRotating = false;
                StartCoroutine(ResetRotateCooldown());
            }
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
        }
    }

    void OnRotateRight()
    {
        if (canRotate && !isRotating)
        {
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

    public void TakeKnockback(Vector2 direction, float force)
    {
        isKnockedBack = true;
        rb.linearVelocity = direction * force;
    }
}