using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float maxSpeed = 5f; // Maximum speed the player can reach
    public float maxBoostingSpeed = 15f;
    public float boostOrthMult = 1.25f;
    public float boostOrthChangeTime = 0.1f;
    public float boostShakeAmplitude = 2f;
    public float boostShakeFrequency = 5f;
    public float defaultAcceleration = 50f; // How quickly the player accelerates
    public float defaultDecceleration = 30f; // How quickly the player slows down
    public float collisionOffset = 0.05f;
    public float rotateCooldown = 0.5f;
    public float rotationSpeed = 5f;
    public float knockbackDrag = 5f;
    public ContactFilter2D movementFilter;

    private float acceleration;
    public float Acceleration
    {
        get => acceleration;
        set => acceleration = value;
    }
    public float Deceleration
    {
        get => deceleration;
        set => deceleration = value;
    }
    private float deceleration;
    private Vector2 movementInput;
    public float CurPlayerSpeed => currentVelocity.magnitude;
    private Vector2 currentVelocity;
    private Rigidbody2D rb;
    private List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    private bool canRotate;
    private bool isRotating = false;
    private float targetAngle;
    private bool isKnockedBack;
    private bool isBoosting;
    private PlayerBoosterController[] boosters;
    private CameraManager cameraManager;
    private PlayerGunController[] guns;
    private PauseManager pauseManager;
    private SideEffectsManager sideEffectsManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        acceleration = defaultAcceleration;
        deceleration = defaultDecceleration;
        canRotate = true;
        isRotating = false;
        isKnockedBack = false;
        isBoosting = false;
        boosters = GetComponentsInChildren<PlayerBoosterController>(true);
        movementFilter.useLayerMask = true;
        movementFilter.SetLayerMask(LayerMask.GetMask("border"));
        cameraManager = FindAnyObjectByType<CameraManager>();
        guns = GetComponentsInChildren<PlayerGunController>(true);
        pauseManager = FindAnyObjectByType<PauseManager>(FindObjectsInactive.Include);
        sideEffectsManager = FindAnyObjectByType<SideEffectsManager>();
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

        // Boosting
        if (isBoosting)
        {
            Vector2 totalBoost = Vector2.zero;
            foreach (var booster in boosters)
            {
                if (booster != null && !booster.IsDisabled)
                {
                    totalBoost += booster.ThrustDirection * booster.boostForce;

                }
            }

            if (totalBoost != Vector2.zero)
            {
                currentVelocity = Vector2.ClampMagnitude(currentVelocity += totalBoost * Time.fixedDeltaTime, maxBoostingSpeed);
            }
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

    void OnBoost(InputValue value)
    {
        if (!(boosters.Length == 0))
        {
            isBoosting = value.isPressed;
            cameraManager.ChangeOrthSize(boostOrthMult, boostOrthChangeTime, !isBoosting);
            cameraManager.ShakeCamera(boostShakeAmplitude, boostShakeFrequency, isBoosting);
            foreach (var booster in boosters)
            {
                booster.UseBoostingSprite(isBoosting);
            }
        }
    }

    void OnAutoShoot(InputValue value)
    {
        foreach(var gun in guns)
        {
            gun.IsAutoShooting = value.isPressed;
        }
    }

    void OnManualShoot()
    {
        foreach (var gun in guns)
        {
            gun.TryManualShot();
        }
    }

    void OnPause()
    {
        pauseManager.TogglePause();
    }

    IEnumerator ResetRotateCooldown()
    {
        yield return new WaitForSeconds(rotateCooldown);
        canRotate = true;
    }

    public void TakeKnockback(Vector2 direction, float force)
    {
        force *= sideEffectsManager.CurKnockbackMult;
        isKnockedBack = true;
        rb.linearVelocity = direction * force;
    }
}