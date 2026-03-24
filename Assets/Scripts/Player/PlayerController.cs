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
    public float defaultDeceleration = 30f; // How quickly the player slows down
    public float collisionOffset = 0.1f;
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
    public bool IsBoosting
    {
        get => isBoosting;
        set => isBoosting = value;
    }
    private PlayerBoosterController[] boosters;
    private CameraManager cameraManager;
    private PlayerGunController[] guns;
    private PauseManager pauseManager;
    private SideEffectsManager sideEffectsManager;
    private bool noWeaponsEffect = false;
    public bool NoWeaponsEffect
    {
        get => noWeaponsEffect;
        set => noWeaponsEffect = value;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        acceleration = defaultAcceleration;
        deceleration = defaultDeceleration;
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
        Vector2 knockbackVelocity = Vector2.zero;
        if (isKnockedBack)
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, knockbackDrag * Time.fixedDeltaTime);

            if (rb.linearVelocity.magnitude < 0.1f)
            {
                rb.linearVelocity = Vector2.zero;
                isKnockedBack = false;
            }

            knockbackVelocity = rb.linearVelocity;
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

        // Boosting
        Vector2 totalBoost = Vector2.zero;
        if (isBoosting)
        {
            foreach (var booster in boosters)
            {
                if (booster != null && !booster.IsDisabled)
                {
                    totalBoost += booster.ThrustDirection * booster.boostForce;
                }
            }
        }

        // Calculate the new position
        Vector2 finalVelocity = Vector2.ClampMagnitude(currentVelocity + knockbackVelocity + totalBoost, maxBoostingSpeed);
        float distance = finalVelocity.magnitude * Time.fixedDeltaTime;

        // Check for collisions using raycast
        int count = rb.Cast(
            finalVelocity.normalized,
            movementFilter,
            castCollisions,
            distance + collisionOffset
        );

        // Move if no collisions detected
        if (count == 0)
        {
            // newPosition
            rb.MovePosition(rb.position + finalVelocity * Time.fixedDeltaTime);
            currentVelocity = finalVelocity;
        }
        else
        {
            // Find closest hit
            RaycastHit2D closestHit = castCollisions[0];
            foreach (var hit in castCollisions)
            {
                if (hit.distance < closestHit.distance)
                {
                    closestHit = hit;
                }
            }

            float safeDistance = Mathf.Max(closestHit.distance - collisionOffset, 0f);

            // Move up to wall
            rb.MovePosition(rb.position + finalVelocity.normalized * safeDistance);

            Vector2 normal = closestHit.normal;

            // Only remove velocity INTO the wall
            float dot = Vector2.Dot(finalVelocity, normal);
            if (dot < 0)
            {
                finalVelocity -= dot * normal;
            }

            // Small push away to prevent sticking
            rb.MovePosition(rb.position + normal * 0.001f);

            currentVelocity = finalVelocity;
            rb.linearVelocity = finalVelocity;
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
            ToggleAllEnemyColours();
        }
    }

    void OnRotateRight()
    {
        if (canRotate && !isRotating)
        {
            targetAngle = transform.eulerAngles.z - 90;
            isRotating = true;
            canRotate = false;
            ToggleAllEnemyColours();
        }
    }

    private void ToggleAllEnemyColours()
    {
        EnemyData[] enemies = FindObjectsByType<EnemyData>(FindObjectsSortMode.None);

        foreach (EnemyData enemy in enemies)
        {
            if (enemy != null && !enemy.IsDead)
            {
                enemy.ToggleColour();
            }
        }
    }

    void OnBoost(InputValue value)
    {
        if (!(boosters.Length == 0) && noWeaponsEffect == false)
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
        if (noWeaponsEffect == false)
        {
            foreach(var gun in guns)
            {
                gun.IsAutoShooting = value.isPressed;
            }   
        }
    }

    void OnManualShoot()
    {
        if (noWeaponsEffect == false)
        {
            foreach (var gun in guns)
            {
                gun.TryManualShot();
            }
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