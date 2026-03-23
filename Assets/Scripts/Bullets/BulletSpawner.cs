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
    [SerializeField] private bool overrideBulletSize = false;
    [SerializeField] private float bulletSizeOverride = 1f;

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
    [SerializeField] private float sweepActiveTime = 2f;
    [SerializeField] private float sweepPauseTime = 1f;

    [Header("Random Arc Pattern")]
    [SerializeField] private float randomArcAngle = 90f;
    [SerializeField] private float timeBetweenRandomShots = 0.08f;

    private Coroutine firingRoutine;
    private int sweepIndex = 0;
    private int sweepDirection = 1;
    private bool isBlue;

    private IEnumerator Start()
    {
        EnemyData enemyData = GetComponent<EnemyData>();
        isBlue = enemyData.IsBlue;
        bulletType = isBlue ? Bullet.BulletType.BlueBullet : Bullet.BulletType.RedBullet;

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
                yield return new WaitForSeconds(timeBetweenBurstShots);
        }

        yield return new WaitForSeconds(timeBetweenBursts);
    }

    private IEnumerator SweepArcRoutine()
    {
        float elapsed = 0f;

        while (elapsed < sweepActiveTime)
        {
            FireSweepBullet();
            yield return new WaitForSeconds(timeBetweenSweepShots);
            elapsed += timeBetweenSweepShots;
        }

        yield return new WaitForSeconds(sweepPauseTime);
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

        float t = sweepSteps > 1 ? (float)sweepIndex / (sweepSteps - 1) : 0f;

        float localAngle = Mathf.Lerp(-halfArc, halfArc, t);
        Vector2 direction = RotateVector(GetAimDirection(), localAngle);

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

    // FIXED: always use forward (matches your rotation system)
    private Vector2 GetAimDirection()
    {
        return -firePoint.up;
    }

    private void FireBulletInDirection(Vector2 direction)
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        //  Rotate bullet to match direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bulletObj.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet != null)
        {
            if (overrideBulletSize)
                bullet.Initialize(direction, bulletSpeed, bulletLifetime, bulletType, bulletSizeOverride);
            else
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