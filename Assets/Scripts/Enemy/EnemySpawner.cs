using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float baseSpawnInterval = 1.5f;
    [SerializeField] private float minSpawnInterval = 0.3f;
    [SerializeField] private float spawnRampDuration = 120f;
    [SerializeField] private float spawnRadius = 12f;
    [SerializeField] private float baseEnemySpeed = 2f;
    [SerializeField] private float maxEnemySpeed = 5f;
    [SerializeField] private int maxWaveSize = 4;

    private Transform player;
    private float spawnTimer;
    private float elapsedTime;

    private void Start()
    {
        var p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
    }

    private void Update()
    {
        if (player == null || enemyPrefab == null) return;

        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / spawnRampDuration);
        float interval = Mathf.Lerp(baseSpawnInterval, minSpawnInterval, t);

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= interval)
        {
            spawnTimer = 0f;
            int waveSize = Mathf.RoundToInt(Mathf.Lerp(1, maxWaveSize, t));
            float speed = Mathf.Lerp(baseEnemySpeed, maxEnemySpeed, t);
            for (int i = 0; i < waveSize; i++)
                SpawnEnemy(speed);
        }
    }

    private void SpawnEnemy(float speed)
    {
        Vector2 origin = player != null ? (Vector2)player.position : Vector2.zero;
        Vector2 spawnPos = origin + Random.insideUnitCircle.normalized * spawnRadius;
        var go = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        go.GetComponent<EnemyController>()?.Init(player, speed);
    }
}
