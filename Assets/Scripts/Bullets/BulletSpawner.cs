using System.Collections;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public enum FirePattern
    {
        Burst,
        SweepArc,
        RandomArc
    }

    [Header("References")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform player;

    [Header("General Bullet Settings")]
    [SerializeField] private float bulletSpeed = 8f;
    [SerializeField] private float bulletLifetime = 5f;
    [SerializeField] private Bullet.BulletType bulletType = Bullet.BulletType.BlueBullet;
    [SerializeField] private bool startFiringOnStart = true;

    [Header("Pattern Selection")]
    [SerializeField] private FirePattern firePattern = FirePattern.Burst;

    [Header("Burst Pattern")]
    [SerializeField] private int bulletsPerBurst = 3;
    [SerializeField] private float timeBetweenBurstShots = 0.12f;
    [SerializeField] private float timeBetweenBursts = 1.2f;

    [Header("Sweep Arc Pattern")]
    [SerializeField] private float sweepArcAngle = 180f;
    [SerializeField] private int sweepSteps = 12;
    [SerializeField] private float timeBetweenSweepShots = 0.08f;
    [SerializeField] private bool sweepContinuously = true;

    [Header("Random Arc Pattern")]
    [SerializeField] private float randomArcAngle = 90f;
    [SerializeField] private float timeBetweenRandomShots = 0.08f;

    private Coroutine firingRoutine;
    private int sweepIndex = 0;
    private int sweepDirection = 1;

    private IEnumerator Start()
    {
        if (startFiringOnStart)
        {
            yield return null;
            StartFiring();
        }
    }

    public void StartFiring()
    {
        if (firingRoutine == null)
        {
            firingRoutine = StartCoroutine(FiringRoutine());
        }
    }

    public void StopFiring()
    {
        if (firingRoutine != null)
        {
            StopCoroutine(firingRoutine);
            firingRoutine = null;
        }
    }

    private IEnumerator FiringRoutine()
    {
        while (true)
        {
            switch (firePattern)
            {
                case FirePattern.Burst:
                    yield return StartCoroutine(BurstRoutine());
                    break;

                case FirePattern.SweepArc:
                    yield return StartCoroutine(SweepArcRoutine());
                    break;

                case FirePattern.RandomArc:
                    yield return StartCoroutine(RandomArcRoutine());
                    break;
            }
        }
    }

    private IEnumerator BurstRoutine()
    {
        for (int i = 0; i < bulletsPerBurst; i++)
        {
            FireBulletInDirection(GetAimDirection());

            if (i < bulletsPerBurst - 1)
            {
                yield return new WaitForSeconds(timeBetweenBurstShots);
            }
        }

        yield return new WaitForSeconds(timeBetweenBursts);
    }

    private IEnumerator SweepArcRoutine()
    {
        FireSweepBullet();
        yield return new WaitForSeconds(timeBetweenSweepShots);

        if (!sweepContinuously)
        {
            yield return null;
        }
    }

    private IEnumerator RandomArcRoutine()
    {
        FireRandomArcBullet();
        yield return new WaitForSeconds(timeBetweenRandomShots);
    }

    private void FireSweepBullet()
    {
        if (firePoint == null) return;

        float halfArc = sweepArcAngle * 0.5f;

        float t = 0f;
        if (sweepSteps > 1)
        {
            t = (float)sweepIndex / (sweepSteps - 1);
        }

        float localAngle = Mathf.Lerp(-halfArc, halfArc, t);
        Vector2 direction = RotateVector(firePoint.right, localAngle);

        FireBulletInDirection(direction);

        sweepIndex += sweepDirection;

        if (sweepIndex >= sweepSteps - 1)
        {
            sweepIndex = sweepSteps - 1;
            sweepDirection = -1;
        }
        else if (sweepIndex <= 0)
        {
            sweepIndex = 0;
            sweepDirection = 1;
        }
    }

    private void FireRandomArcBullet()
    {
        if (firePoint == null) return;

        Vector2 baseDirection = GetAimDirection();
        float halfArc = randomArcAngle * 0.5f;
        float randomAngle = Random.Range(-halfArc, halfArc);

        Vector2 direction = RotateVector(baseDirection, randomAngle);
        FireBulletInDirection(direction);
    }

    private Vector2 GetAimDirection()
    {
        if (player != null)
        {
            return ((Vector2)(player.position - firePoint.position)).normalized;
        }

        return firePoint.right;
    }

    private void FireBulletInDirection(Vector2 direction)
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.Initialize(direction, bulletSpeed, bulletLifetime, bulletType);
        }
    }

    private Vector2 RotateVector(Vector2 vector, float angleDegrees)
    {
        float radians = angleDegrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);

        return new Vector2(
            vector.x * cos - vector.y * sin,
            vector.x * sin + vector.y * cos
        ).normalized;
    }
}