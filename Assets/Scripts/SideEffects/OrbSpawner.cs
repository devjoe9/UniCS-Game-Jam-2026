using UnityEngine;

public class OrbSpawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject orbPrefab;

    [Header("Spawn Boundary")]
    public BoxCollider2D spawnBoundary;

    [Header("Spawn Settings")]
    public float spawnInterval = 8f;
    public int   maxOrbs       = 3;
    public float edgePadding   = 0.5f;

    private int   activeOrbCount = 0;
    private float timer          = 0f;

    private void Start()
    {
        if (spawnBoundary == null) { Debug.LogError("[OrbSpawner] No spawn boundary assigned!"); enabled = false; return; }
        if (orbPrefab     == null) { Debug.LogError("[OrbSpawner] No orb prefab assigned!");     enabled = false; return; }
        TrySpawnOrb();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            TrySpawnOrb();
        }
    }

    private void TrySpawnOrb()
    {
        if (activeOrbCount >= maxOrbs) return;

        Vector2    pos = GetRandomPositionInBounds();
        GameObject orb = Instantiate(orbPrefab, pos, Quaternion.identity);

        HealingOrb script = orb.GetComponent<HealingOrb>();
        if (script != null)
            script.OnPickedUp += OnOrbPickedUp;

        activeOrbCount++;
        Debug.Log($"[OrbSpawner] Spawned at {pos} | Active: {activeOrbCount}/{maxOrbs}");
    }

    private Vector2 GetRandomPositionInBounds()
    {
        Bounds b = spawnBoundary.bounds;
        return new Vector2(
            Random.Range(b.min.x + edgePadding, b.max.x - edgePadding),
            Random.Range(b.min.y + edgePadding, b.max.y - edgePadding));
    }

    private void OnOrbPickedUp()
    {
        activeOrbCount = Mathf.Max(0, activeOrbCount - 1);
        Debug.Log($"[OrbSpawner] Orb picked up. Active: {activeOrbCount}/{maxOrbs}");
    }
}
