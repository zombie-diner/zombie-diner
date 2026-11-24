using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaveSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] ZombieAI normalZombiePrefab;
    [SerializeField] ZombieAI explodingZombiePrefab;
    [SerializeField] ZombieAI foodZombiePrefab;

    [Header("Spawn Points")]
    [SerializeField] Transform[] spawnPoints;

    [Header("Normal Zombies")]
    [SerializeField] int startNormalCount = 4;
    [SerializeField] int normalPerWave = 2;

    [Header("Exploding Zombies")]
    [SerializeField] int explodingStartWave = 6;
    [SerializeField] int explodingPerWave = 1;

    [Header("Food Zombies")]
    [SerializeField] int foodZomStartWave = 4;
    [SerializeField] int foodZomPerWave = 1;

    [Header("Pacing")]
    [SerializeField] float spawnInterval = 1.5f;
    [SerializeField] int maxAlive = 8;

    [Header("Testing (until GameManager exists)")]
    [SerializeField] bool autoStartNextWave = true;
    [SerializeField] float timeBetweenWaves = 5f;

    public int CurrentWave { get; private set; }
    public bool WaveActive { get; private set; }
    public int AliveCount => alive.Count;

    public event Action<int> OnWaveStarted;
    public event Action<int> OnWaveCleared;

    readonly HashSet<ZombieAI> alive = new HashSet<ZombieAI>();
    readonly List<ZombieAI> spawnQueue = new List<ZombieAI>();
    Coroutine spawnRoutine;

    void OnEnable()  => ZombieAI.ZombieDied += HandleZombieDied;
    void OnDisable() => ZombieAI.ZombieDied -= HandleZombieDied;

    void Start()
    {
        if (autoStartNextWave) StartNextWave();
    }

    // ---- Public API: the GameManager will call these later ----

    public void StartNextWave() => StartWave(CurrentWave + 1);

    public void StartWave(int wave)
    {
        if (WaveActive) return;

        CurrentWave = wave;
        WaveActive = true;
        BuildSpawnQueue(wave);

        Debug.Log($"Wave {wave}: {NormalCount(wave)} normal, {ExplodingCount(wave)} exploding, {FoodZomCount(wave)} food");
        OnWaveStarted?.Invoke(wave);
        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    public void StopWave()
    {
        CancelInvoke(nameof(StartNextWave));
        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        spawnQueue.Clear();
        WaveActive = false;
    }

    // ---- Wave math ----

    int NormalCount(int wave) => startNormalCount + normalPerWave * (wave - 1);

    int ExplodingCount(int wave) =>
        wave < explodingStartWave ? 0 : explodingPerWave * (wave - explodingStartWave + 1);

    int FoodZomCount(int wave) =>
        wave < foodZomStartWave ? 0 : foodZomPerWave * (wave - foodZomStartWave + 1);

    void BuildSpawnQueue(int wave)
    {
        spawnQueue.Clear();
        for (int i = 0; i < NormalCount(wave); i++) spawnQueue.Add(normalZombiePrefab);
        for (int i = 0; i < ExplodingCount(wave); i++) spawnQueue.Add(explodingZombiePrefab);
        for (int i = 0; i < FoodZomCount(wave); i++) spawnQueue.Add(foodZombiePrefab);

        // Shuffle so exploders are mixed in
        for (int i = spawnQueue.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (spawnQueue[i], spawnQueue[j]) = (spawnQueue[j], spawnQueue[i]);
        }
    }

    // ---- Spawning ----

    IEnumerator SpawnLoop()
    {
        while (spawnQueue.Count > 0)
        {
            // Wait if too many are already in the kitchen
            while (alive.Count >= maxAlive) yield return null;

            ZombieAI prefab = spawnQueue[spawnQueue.Count - 1];
            spawnQueue.RemoveAt(spawnQueue.Count - 1);
            Spawn(prefab);

            yield return new WaitForSeconds(spawnInterval);
        }
        spawnRoutine = null;
        CheckWaveCleared();
    }

    void Spawn(ZombieAI prefab)
    {
        Transform point = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];

        // Make sure the zombie lands on the NavMesh
        if (!NavMesh.SamplePosition(point.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            Debug.LogWarning($"Spawn point {point.name} is not near the NavMesh");
            return;
        }

        ZombieAI zombie = Instantiate(prefab, hit.position, point.rotation);
        alive.Add(zombie);
    }

    void HandleZombieDied(ZombieAI zombie)
    {
        // Only count zombies this spawner made (ignores ones placed by hand in the scene)
        if (alive.Remove(zombie)) CheckWaveCleared();
    }

    void CheckWaveCleared()
    {
        if (!WaveActive || spawnQueue.Count > 0 || alive.Count > 0) return;

        WaveActive = false;
        if (spawnRoutine != null) { StopCoroutine(spawnRoutine); spawnRoutine = null; }
        Debug.Log($"Wave {CurrentWave} cleared!");
        OnWaveCleared?.Invoke(CurrentWave);

        if (autoStartNextWave) Invoke(nameof(StartNextWave), timeBetweenWaves);
    }
}