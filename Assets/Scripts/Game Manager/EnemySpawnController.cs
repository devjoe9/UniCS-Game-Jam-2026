using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    public EnemyInfo[] enemies;
    [SerializeField] private float startingSpawnInterval = 2f;
    [SerializeField] private float spawnIntervalDecreasePerWave = 0.25f;
    [SerializeField] private float minSpawnInterval = 0.2f;
    [SerializeField] private int startingSpawnBudget = 10;
    [SerializeField] private int budgetIncreasePerWave = 5;
    [SerializeField] private float startingWaveInterval = 30f;
    [SerializeField] private float waveIntervalDecresePerWave = 2f;
    [SerializeField] private float minWaveInterval = 20f;
    private Bounds spawnBounds;
    private Camera cam;
    private int currentWave;
    private bool gameOver;
    public int curScore;
    public int CurScore
    {
        get => curScore;
        set => curScore += value;
    }

    // public enum Rarity {Common = 50, Uncommon = 30, Rare = 15}

    [Serializable]
    public class EnemyInfo
    {
        public GameObject enemyPrefab;
        public float spawningWeight; // higher means more likely
        public int spawnCost;
        public int pointsValue;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        spawnBounds = col.bounds;
        cam = FindAnyObjectByType<Camera>();
        currentWave = 0;
        gameOver = false;
        curScore = 0;

        StartCoroutine(WaveLoop());
    }

    void SpawnEnemy(GameObject enemy, Vector3 spawnPos, int points)
    {
        GameObject obj = Instantiate(enemy, spawnPos, Quaternion.identity);
        obj.GetComponent<EnemyData>().PointsValue = points;
    }

    EnemyInfo GetRandomEnemy(List<EnemyInfo> list)
    {
        float total = 0;
        foreach (var enemy in list)
        {
            total += enemy.spawningWeight;
        }
        float randomPoint = UnityEngine.Random.value * total;
        foreach (var enemy in list)
        {
            if (randomPoint <= enemy.spawningWeight)
            {
                return enemy;
            }

            randomPoint -= enemy.spawningWeight;
        }

        return enemies[0]; // just in case
    }

    Vector3 GetRandomSpawnPos(bool outsideCam = false)
    {
        float leftSpawnEdge = spawnBounds.min.x; // Left side x-coordinate
        float rightSpawnEdge = spawnBounds.max.x; // Right side x-coordinate
        float bottomSpawnEdge = spawnBounds.min.y; // Bottom side y-coordinate
        float topSpawnEdge = spawnBounds.max.y; // Top side y-coordinate

        if (outsideCam)
        {
            float height = cam.orthographicSize * 2f;
            float width = height * cam.aspect;
            Vector3 center = cam.transform.position;

            float leftCamEdge = center.x - width / 2f;
            float rightCamEdge = center.x + width / 2f;
            float topCamEdge = center.y + height / 2f;
            float bottomCamEdge = center.y - height / 2f;

            int side = UnityEngine.Random.Range(0, 4);

            switch (side)
            {
                case 0: // left
                    return new Vector3(
                        UnityEngine.Random.Range(leftSpawnEdge, leftCamEdge),
                        UnityEngine.Random.Range(bottomSpawnEdge, topSpawnEdge),
                        0f);

                case 1: // right
                    return new Vector3(
                        UnityEngine.Random.Range(rightCamEdge, rightSpawnEdge),
                        UnityEngine.Random.Range(bottomSpawnEdge, topSpawnEdge),
                        0f);

                case 2: // top
                    return new Vector3(
                        UnityEngine.Random.Range(leftSpawnEdge, rightSpawnEdge),
                        UnityEngine.Random.Range(topCamEdge, topSpawnEdge),
                        0f);

                case 3: // bottom
                    return new Vector3(
                        UnityEngine.Random.Range(leftSpawnEdge, rightSpawnEdge),
                        UnityEngine.Random.Range(bottomSpawnEdge, bottomCamEdge),
                        0f);
            }
        }

        float randomX = UnityEngine.Random.Range(leftSpawnEdge, rightSpawnEdge);
        float randomY = UnityEngine.Random.Range(bottomSpawnEdge, topSpawnEdge);

        return new Vector3(randomX, randomY, 0);
    }

    List<EnemyInfo> GenerateWave(int budget)
    {
        List<EnemyInfo> wave = new List<EnemyInfo>();

        while (budget > 0)
        {
            List<EnemyInfo> validEnemies = new List<EnemyInfo>();

            foreach (var enemy in enemies)
            {
                if (enemy.spawnCost <= budget) validEnemies.Add(enemy);
            }

            if (validEnemies.Count == 0) break;

            EnemyInfo chosen = GetRandomEnemy(validEnemies);
            wave.Add(chosen);
            budget -= chosen.spawnCost;
        }

        return wave;
    }

    IEnumerator SpawnWave(List<EnemyInfo> wave, float timeBetweenSpawns)
    {
        foreach (var enemy in wave)
        {
            SpawnEnemy(enemy.enemyPrefab, GetRandomSpawnPos(), enemy.pointsValue);
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    IEnumerator WaveLoop()
    {
        while (!gameOver)
        {
            int budget = startingSpawnBudget + currentWave * budgetIncreasePerWave;
            float timeBetweenSpawns = Mathf.Max(startingSpawnInterval - currentWave * spawnIntervalDecreasePerWave, minSpawnInterval);
            float timeBetweenWaves = Mathf.Max(startingWaveInterval - currentWave * waveIntervalDecresePerWave, minWaveInterval);

            List<EnemyInfo> wave = GenerateWave(budget);

            yield return StartCoroutine(SpawnWave(wave, timeBetweenSpawns));

            yield return StartCoroutine(WaitForNextWave(timeBetweenWaves));

            currentWave++;
        }
    }

    IEnumerator WaitForNextWave(float duration)
    {
        float timeElapsed = 0;
        while (timeElapsed < duration && GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
}
