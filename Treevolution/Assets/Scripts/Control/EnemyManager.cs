using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Responsible for spawning and managing enemies in the scene
/// </summary>
public class EnemyManager : MonoBehaviour
{
    /// <summary>
    /// List of enemies currently spawned in the game.
    /// </summary>
    private List<GameObject> _enemies = new List<GameObject>();
    /// <summary>
    /// Publicly accessible list of enemy GameObjects currently alive in-game.
    /// </summary>
    public GameObject[] enemies { get { return _enemies.ToArray(); } }
    /// <summary>
    /// Prefab object of ant bug.
    /// </summary>
    public GameObject antPrefab;
    /// <summary>
    /// Prefab object of armoured bug.
    /// </summary>
    public GameObject armouredBugPrefab;
    /// <summary>
    /// Prefab object of cockroach bug.
    /// </summary>
    public GameObject armouredCockroachPrefab;
    /// <summary>
    /// Prefab object of stag beetle bug.
    /// </summary>
    public GameObject armouredStagbeetlePrefab;
    /// <summary>
    /// Prefab object of dragonfly bug.
    /// </summary>
    public GameObject DragonflyPrefab;
    /// <summary>
    /// Prefab object of hornet bug.
    /// </summary>
    public GameObject HornetPrefab;
    /// <summary>
    /// Audio clip to play from an enemy when it spawns.
    /// </summary>
    [SerializeField]
    private AudioClip _spawnAudio;
    /// <summary>
    /// Audio clip to play from an enemy when it dies.
    /// </summary>
    [SerializeField]
    private AudioClip _deathAudio;
    /// <summary>
    /// Audio clip to play from an enemy when it gets damaged.
    /// </summary>
    [SerializeField]
    private AudioClip _damageAudio;
    /// <summary>
    /// Active rountimer reference.
    /// </summary>
    private RoundTimer roundTimer;
    /// <summary>
    /// Number of enemies killed in total.
    /// </summary>
    private int enemiesKilled = 0;
    private int score = 0;
    private bool _enemiesTakeDamage = true;
    /// <summary>
    /// Timer to display in the UI.
    /// </summary>
    private float timer = 0;
    /// <summary>
    /// Default spawn interval in seconds (can be adjusted in Inspector).
    /// </summary>
    public float spawnInterval = 3;
    /// <summary>
    /// Spawn height of enemies, obtained from the game properties.
    /// </summary>
    private float spawnHeight;
    /// <summary>
    /// Vectors denoting the origin, and lateral axes of each spawning region.
    /// </summary>
    private (Vector3 origin, Vector3 vert, Vector3 horz)[] spawnVectors = new (Vector3 origin, Vector3 vert, Vector3 horz)[2];
    /// <summary>
    /// Pool of all enemy types to be spawned in a round.
    /// </summary>
    private List<GameStateManager.EnemyType> spawnPool;
    /// <summary>
    /// Indicates whether spawning has started.
    /// </summary>
    private bool started = false;
    /// <summary>
    /// Indicates whether the first enemy spawn has happened (for tutorial pop-ups).
    /// </summary>
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

    /// <summary>
    /// Gets the total number of enemies killed.
    /// </summary>
    /// <returns>Integer number of enemies killed.</returns>
    public int getEnemiesKilled()
    {
        return enemiesKilled;
    }


    public int getScore()
    {
        return score;
    }


    /// <summary>
    /// Resets the number of enemies killed.
    /// </summary>
    public void resetEnemiesKilled()
    {
        enemiesKilled = 0;
        score = 0;
    }

    /// <summary>
    /// Gets the enemy prefab of a particular enemy type.
    /// </summary>
    /// <param name="enemyType">The enemy type to get the prefab of.</param>
    /// <returns>The prefab of the enemy type requested.</returns>
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

    /// <summary>
    /// Spawns a random enemy from the enemypool at a random position within either spawn region.
    /// </summary>
    void spawnEnemy()
    {
        if (spawnPool.Count == 0)
            return;
        int randI = Random.Range(0, spawnPool.Count);
        GameStateManager.EnemyType enemyType = spawnPool[randI];
        spawnPool.RemoveAt(randI);
        GameObject enemyPrefab = getEnemyPrefab(enemyType);
        EnemyScript enemyPrefabScript = enemyPrefab.GetComponent<EnemyScript>();
        enemyPrefabScript.damageAudio = _damageAudio;
        enemyPrefabScript.spawnAudio = _spawnAudio;
        enemyPrefabScript.deathAudio = _deathAudio;

        int LR = Random.Range(0, 2); // Random
        (Vector3 origin, Vector3 vert, Vector3 horz) spawnAxes = spawnVectors[LR]; // chooses either the left or right edge to spawn from.
        float vFraction = Random.value; // vertical fraction
        float hFraction = Random.value; // horizontal fraction

        Vector3 randomSpawnPosition = spawnAxes.origin + spawnAxes.vert * vFraction + spawnAxes.horz * hFraction;
        randomSpawnPosition.y = spawnHeight;
        GameObject newEnemy = Instantiate(enemyPrefab, randomSpawnPosition, enemyPrefab.transform.rotation);
        lock (enemies)
        {
            _enemies.Add(newEnemy);
            newEnemy.GetComponent<EnemyScript>().takeDamage = _enemiesTakeDamage;
        }
        Debug.DrawLine(spawnAxes.origin, spawnAxes.origin + spawnAxes.vert, Color.white, 1000);
        Debug.DrawLine(spawnAxes.origin, spawnAxes.origin + spawnAxes.horz, Color.white, 1000);
    }

    /// <summary>
    /// Adds enemies to the scene for testing purposes.
    /// </summary>
    /// <param name="enemy">GameObject to spawn as an enemy.</param>
    public void AddToSceneAsEnemyForTest(GameObject enemy)
    {
        _enemies.Add(enemy);
    }

    /// <summary>
    /// Unpacks the dictionary of enemy types to spawn in a round, with their count, into a single pool of enemy types.
    /// </summary>
    /// <param name="enemies"></param>
    /// <returns></returns>
    private List<GameStateManager.EnemyType> unpackEnemies(Dictionary<GameStateManager.EnemyType, int> enemies)
    {
        List<GameStateManager.EnemyType> enemyPool = new List<GameStateManager.EnemyType>();
        foreach (GameStateManager.EnemyType enemyType in enemies.Keys)
        {
            enemyPool.AddRange(Enumerable.Repeat(enemyType, enemies[enemyType]));
        }
        return enemyPool;
    }

    /// <summary>
    /// Begins spawning enemies.
    /// </summary>
    /// <param name="enemies">Dictionary of enemy types, along with a count of how many of each type to spawn.</param>
    public void StartSpawning(Dictionary<GameStateManager.EnemyType, int> enemies)
    {
        spawnPool = unpackEnemies(enemies);
        spawnInterval = (roundTimer.GetRoundLength() * 0.8f) / enemies.Values.Sum();
        Vector3 verticalLeft = GameProperties.BottomLeftCorner - GameProperties.TopLeftCorner;
        Vector3 horizontalLeft = (GameProperties.TopRightCorner - GameProperties.TopLeftCorner) * 0.1f;
        Vector3 verticalRight = GameProperties.BottomRightCorner - GameProperties.TopRightCorner;
        Vector3 horizontalRight = (GameProperties.TopLeftCorner - GameProperties.TopRightCorner) * 0.1f;
        spawnVectors[0] = (GameProperties.TopLeftCorner, verticalLeft, horizontalLeft);
        spawnVectors[1] = (GameProperties.TopRightCorner, verticalRight, horizontalRight);
        spawnHeight = 0.01f + GameProperties.FloorHeight;
        started = true;
    }

    /// <summary>
    /// Removes an enemy from the game.
    /// </summary>
    /// <param name="enemy">The enemy GameObject to remove.</param>
    /// <param name="killedByPlayer">Indicator of whether that enemy was killed by a player, or if it just reached the tree.</param>
    public void RemoveEnemy(GameObject enemy, bool countPoints)
    {
        if (countPoints)
        {
            score += enemy.GetComponent<EnemyScript>().points;
            enemiesKilled++;
        }
        _enemies.Remove(enemy);
    }

    public void toggleDamage(bool damageOn)
    {
        _enemiesTakeDamage = damageOn;
        lock (enemies)
        {
            foreach (GameObject enemy in enemies)
            {
                enemy.GetComponent<EnemyScript>().takeDamage = _enemiesTakeDamage;
            }
        }
    }

    /// <summary>
    /// Stops the enemies from spawning.
    /// </summary>
    public void StopSpawning()
    {
        started = false;
    }
}
