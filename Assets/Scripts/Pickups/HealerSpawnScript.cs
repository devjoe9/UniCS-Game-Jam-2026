using System.Collections;
using UnityEngine;

public class HealerSpawnScript : MonoBehaviour
{
    public GameObject healerPrefab;
    public GameObject healerSpawnParticlesPrefab;
    public float minSpawnInterval = 10f;
    public float maxSpawnInterval = 15f;
    public float spawnParticlesLifetime;
    private Bounds spawnBounds;
    private Vector2 healerExtents;
    private PlayerData playerData;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BoxCollider2D spawnBox = GetComponent<BoxCollider2D>();
        if (spawnBox == null)
        {
            Debug.LogError("No spawnBox found");
        }
        spawnBounds = spawnBox.bounds;

        Collider2D healerCollider = healerPrefab.GetComponent<Collider2D>();
        if (healerCollider == null)
        {
            Debug.LogError("No healer collider found");
        }
        healerExtents = healerCollider.bounds.extents;

        playerData = FindFirstObjectByType<PlayerData>();

        StartCoroutine(SpawnHealer(minSpawnInterval, maxSpawnInterval));
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator SpawnHealer(float minSpawnInterval, float maxSpawnInterval)
    {
        while (!playerData.IsDead) // change to check player still alive?
        {
            yield return new WaitForSeconds(Random.Range(minSpawnInterval, maxSpawnInterval));
            if (!playerData.isMaxHealth())
            { 
                Vector3 spawnPos = GetRandomSpawnPosition(spawnBounds);
                Instantiate(healerSpawnParticlesPrefab, spawnPos, Quaternion.identity);
                yield return new WaitForSeconds(spawnParticlesLifetime);
                Instantiate(healerPrefab, spawnPos, Quaternion.identity);
            }
        }
    }

    Vector3 GetRandomSpawnPosition(Bounds spawnBounds)
    {
        // All edges account for healer bounds
        float leftEdge = spawnBounds.min.x + healerExtents.x; // Left side x-coordinate
        float rightEdge = spawnBounds.max.x - healerExtents.x; // Right side x-coordinate
        float bottomEdge = spawnBounds.min.y + healerExtents.y; // Bottom side y-coordinate
        float topEdge = spawnBounds.max.y - healerExtents.y; // Top side y-coordinate

        float randomX = Random.Range(leftEdge, rightEdge);
        float randomY = Random.Range(bottomEdge, topEdge);

        return new Vector3(randomX, randomY, 0);
    }
}
