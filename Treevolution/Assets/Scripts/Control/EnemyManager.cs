using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private List<GameObject> _enemies = new List<GameObject>();
    public GameObject[] enemies { get { return _enemies.ToArray(); } }

    public GameObject cubePrefab;
    public float timer = 0;
    public float spawnRate = 1;
    public float coordinate_X = -5;
    public float coordinate_Y = 5;
    float spawnHeight = 0.5f;
    public Vector3 spawnCentre = new Vector3(0, 0, 0);

    bool started = false;
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
            if (timer < spawnRate)
                timer = timer + Time.deltaTime;
            else
            {
                spawnEnemy();
                timer = 0;
            }
        }
    }

    void spawnEnemy(){
        Vector3 randomSpawnPosition = spawnCentre + new Vector3(Random.Range(-coordinate_X, coordinate_X), spawnHeight, Random.Range(-coordinate_Y, coordinate_Y));
        _enemies.Add(Instantiate(cubePrefab, randomSpawnPosition, transform.rotation));
    }

    public void StartSpawning()
    {
        started = true;
        Vector3 vertical = GameProperties.TopLeftCorner - GameProperties.BottomLeftCorner;
        Vector3 horizontal = GameProperties.TopRightCorner - GameProperties.TopLeftCorner;
        spawnCentre = GameProperties.BottomLeftCorner + (0.5f * vertical) + (0.1f * horizontal);
        coordinate_X = horizontal.x * 0.04f;
        coordinate_Y = vertical.z *0.2f;
        spawnHeight = 2f + GameProperties.FloorHeight;
    }
}
