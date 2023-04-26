using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private List<GameObject> _enemies = new List<GameObject>();
    public GameObject[] enemies { get { return _enemies.ToArray(); } }
    private GameObject targetEnemy;
    public GameObject antPrefab;
    public GameObject armouredBugPrefab;
    public GameObject armouredCockroachPrefab;
    public GameObject armouredStagbeetlePrefab;
    public GameObject DragonflyPrefab;
    public GameObject HornetPrefab;

    private RoundTimer roundTimer;
    private int enemiesKilled = 0;
    private float timer = 0;
    public float spawnInterval = 3;
    private float spawnHeight;
    private (Vector3 origin, Vector3 vert, Vector3 horz)[] spawnVectors = new (Vector3 origin, Vector3 vert, Vector3 horz)[2];
    private List<GameStateManager.EnemyType> spawnPool;
    private GameStateManager.EnemyType[] enemyTypesLeft;
    private bool started = false;
    private bool firstSpawn = false;

    // Start is called before the first frame update
    void Start()
    {
        roundTimer = GetComponent<RoundTimer>();
    }

    public void SetupForTest()
    {
        roundTimer = GetComponent<RoundTimer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (started && !roundTimer.isPaused)
        {
            if (!firstSpawn)
            {
                spawnEnemy();
                firstSpawn = true;
            }
            if (timer < spawnInterval)
                timer = timer + Time.deltaTime;
            else if (started)
            {
                spawnEnemy();
                timer = 0;
            }
        }
    }

    public void targetNewEnemy(GameObject enemy)
    {
        if (targetEnemy != null)
        {
            lock (targetEnemy)
            {
                Behaviour oldHalo = (Behaviour)targetEnemy.GetComponent("Halo");
                oldHalo.enabled = false;
            }
        }
        Behaviour newHalo = (Behaviour)enemy.GetComponent("Halo");
        newHalo.enabled = true;
        targetEnemy = enemy;
    }

    public int getEnemiesKilled()
    {
        return enemiesKilled;
    }

    public void resetEnemiesKilled()
    {
        enemiesKilled = 0;
    }

    GameObject getEnemyPrefab(GameStateManager.EnemyType enemyType)
    {
        switch (enemyType)
        {
            case GameStateManager.EnemyType.Ant:
                return antPrefab;
            case GameStateManager.EnemyType.Armoured_Bug:
                return armouredBugPrefab;
            case GameStateManager.EnemyType.Armoured_Cockroach:
                return armouredCockroachPrefab;
            case GameStateManager.EnemyType.Armoured_Stagbeetle:
                return armouredStagbeetlePrefab;
            case GameStateManager.EnemyType.Dragonfly:
                return DragonflyPrefab;
            case GameStateManager.EnemyType.Hornet:
                return HornetPrefab;
        }
        return antPrefab;
    }

    void spawnEnemy()
    {
        if (spawnPool.Count == 0)
            return;
        int randI = Random.Range(0, spawnPool.Count);
        GameStateManager.EnemyType enemyType = spawnPool[randI];
        spawnPool.RemoveAt(randI);
        GameObject enemyPrefab = getEnemyPrefab(enemyType);

        int LR = Random.Range(0, 2); // Random
        (Vector3 origin, Vector3 vert, Vector3 horz) spawnAxes = spawnVectors[LR]; // chooses either the left or right edge to spawn from.
        float vFraction = Random.value; // vertical fraction
        float hFraction = Random.value; // horizontal fraction

        Vector3 randomSpawnPosition = spawnAxes.origin + spawnAxes.vert * vFraction + spawnAxes.horz * hFraction;
        randomSpawnPosition.y = spawnHeight;
        _enemies.Add(Instantiate(enemyPrefab, randomSpawnPosition, enemyPrefab.transform.rotation));
        Debug.DrawLine(spawnAxes.origin, spawnAxes.origin + spawnAxes.vert, Color.white, 1000);
        Debug.DrawLine(spawnAxes.origin, spawnAxes.origin + spawnAxes.horz, Color.white, 1000);
    }

    public void AddToSceneAsEnemyForTest(GameObject enemy)
    {
        _enemies.Add(enemy);
    }

    private List<GameStateManager.EnemyType> unpackEnemies(Dictionary<GameStateManager.EnemyType, int> enemies)
    {
        List<GameStateManager.EnemyType> enemyPool = new List<GameStateManager.EnemyType>();
        foreach (GameStateManager.EnemyType enemyType in enemies.Keys)
        {
            enemyPool.AddRange(Enumerable.Repeat(enemyType, enemies[enemyType]));
        }
        return enemyPool;
    }

    public void StartSpawning(Dictionary<GameStateManager.EnemyType, int> enemies)
    {
        spawnPool = unpackEnemies(enemies);
        spawnInterval = (roundTimer.roundLengthSecs * 0.8f) / enemies.Values.Sum();
        Vector3 verticalLeft = GameProperties.BottomLeftCorner - GameProperties.TopLeftCorner;
        Vector3 horizontalLeft = (GameProperties.TopRightCorner - GameProperties.TopLeftCorner) * 0.1f;
        Vector3 verticalRight = GameProperties.BottomRightCorner - GameProperties.TopRightCorner;
        Vector3 horizontalRight = (GameProperties.TopLeftCorner - GameProperties.TopRightCorner) * 0.1f;
        spawnVectors[0] = (GameProperties.TopLeftCorner, verticalLeft, horizontalLeft);
        spawnVectors[1] = (GameProperties.TopRightCorner, verticalRight, horizontalRight);
        spawnHeight = 0.01f + GameProperties.FloorHeight;
        started = true;
    }

    public void RemoveEnemy(GameObject enemy, bool killedByPlayer)
    {
        if (killedByPlayer)
        {
            enemiesKilled += 1;
        }
        _enemies.Remove(enemy);
    }

    public void StopSpawning()
    {
        started = false;
    }
}
