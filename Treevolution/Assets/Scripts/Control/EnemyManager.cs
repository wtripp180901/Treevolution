using System.Collections;
using System.Collections.Generic;
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

    private int enemiesKilled = 0;
    private float timer = 0;
    public float spawnInterval = 3;
    private float spawnHeight;
    private (Vector3 origin, Vector3 vert, Vector3 horz)[] spawnVectors = new (Vector3 origin, Vector3 vert, Vector3 horz)[2];
    private Dictionary<GameStateManager.EnemyType, int> enemiesLeft;
    bool started = false;
    bool firstSpawn = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (started)
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
        if (targetEnemy != null) {
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
        }
        return antPrefab;
    }

    void spawnEnemy()
    {
        GameStateManager.EnemyType enemyType = (GameStateManager.EnemyType)Random.Range(0, enemiesLeft.Count);
        while (enemiesLeft.Count > 0 && enemiesLeft[enemyType] == 0)
        {
            enemiesLeft.Remove(enemyType);
            enemyType = (GameStateManager.EnemyType)Random.Range(0, enemiesLeft.Count);
        }
        if(enemiesLeft.Count == 0)
        {
            return;
        }
        enemiesLeft[enemyType]--;
        GameObject enemyPrefab = getEnemyPrefab(enemyType);

        int LR = Random.Range(0, 2); // Random
        (Vector3 origin, Vector3 vert, Vector3 horz) spawnAxes = spawnVectors[LR]; // chooses either the left or right edge to spawn from.
        float vFraction = Random.value; // vertical fraction
        float hFraction = Random.value; // horizontal fraction

        Vector3 randomSpawnPosition = spawnAxes.origin + spawnAxes.vert * vFraction + spawnAxes.horz * hFraction;
        randomSpawnPosition.y = spawnHeight;
        _enemies.Add(Instantiate(enemyPrefab, randomSpawnPosition, transform.rotation));
        Debug.DrawLine(spawnAxes.origin, spawnAxes.origin + spawnAxes.vert, Color.white, 1000);
        Debug.DrawLine(spawnAxes.origin, spawnAxes.origin + spawnAxes.horz, Color.white, 1000);
    }

    public void StartSpawning(Dictionary<GameStateManager.EnemyType, int> enemies)
    {
        enemiesLeft = enemies;
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
