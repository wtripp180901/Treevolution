using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private List<GameObject> _enemies = new List<GameObject>();
    public GameObject[] enemies { get { return _enemies.ToArray(); } }
    private GameObject targetEnemy;
    public GameObject enemyPrefab;
    private int enemiesKilled = 0;
    private float timer = 0;
    public float spawnInterval = 3;
    public float coordinate_X = -5;
    public float coordinate_Y = 5;

    float spawnHeight = 0.5f;
    public Vector3 spawnCentre = new Vector3(0, 0, 0);

    private (Vector3 origin, Vector3 vert, Vector3 horz)[] spawnVectors = new (Vector3 origin, Vector3 vert, Vector3 horz)[2];

    bool started = false;
    bool firstSpawn = false;
    public bool DevMode = false;

    // Start is called before the first frame update
    void Start()
    {
        if (DevMode) started = true;
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

            if (targetEnemy != null) { 
            
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

    void spawnEnemy()
    {
        //Vector3 randomSpawnPosition = spawnCentre + new Vector3(Random.Range(-coordinate_X, coordinate_X), spawnHeight, Random.Range(-coordinate_Y, coordinate_Y));

        int LR = Random.Range(0, 2); // Random
        (Vector3 origin, Vector3 vert, Vector3 horz) spawnAxes = spawnVectors[LR]; // chooses either the left or right edge to spawn from.
        float vFraction = Random.value; // vertical fraction
        float hFraction = Random.value; // horizontal fraction

        Vector3 randomSpawnPosition = spawnAxes.origin + spawnAxes.vert * vFraction + spawnAxes.horz * hFraction;
        randomSpawnPosition.y = spawnHeight;
        _enemies.Add(Instantiate(enemyPrefab, randomSpawnPosition, transform.rotation));
    }

    public void StartSpawning()
    {
        started = true;
        Vector3 verticalLeft = GameProperties.BottomLeftCorner - GameProperties.TopLeftCorner;
        Vector3 horizontalLeft = (GameProperties.TopRightCorner - GameProperties.TopLeftCorner) * 0.1f;
        Vector3 verticalRight = GameProperties.BottomRightCorner - GameProperties.TopRightCorner;
        Vector3 horizontalRight = (GameProperties.TopLeftCorner - GameProperties.TopRightCorner) * 0.1f;
        spawnVectors[0] = (GameProperties.TopLeftCorner, verticalLeft, horizontalLeft);
        spawnVectors[1] = (GameProperties.TopRightCorner, verticalRight, horizontalRight);
        //spawnCentre = GameProperties.BottomLeftCorner + (0.5f * vertical) + (0.1f * horizontal);
        //coordinate_X = horizontal.x * 0.04f;
        //coordinate_Y = vertical.z *0.2f;

        spawnHeight = 0.05f + GameProperties.FloorHeight;
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
