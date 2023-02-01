using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private List<EnemyScript> _enemies = new List<EnemyScript>();
    public EnemyScript[] enemies { get { return _enemies.ToArray(); } }

    public GameObject cubePrefab;
    public float timer = 0;
    public float spawnRate = 1;
    public float coordinate_X = -5;
    public float coordinate_Y = 5;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(timer < spawnRate)
            timer = timer + Time.deltaTime;
        else {
            spawnEnemy();
            timer = 0;
        }
    }

    void spawnEnemy(){
        Vector3 randomSpawnPosition = new Vector3(Random.Range(coordinate_X, coordinate_Y), 1, Random.Range(coordinate_X, coordinate_Y));
        Instantiate(cubePrefab, randomSpawnPosition, transform.rotation);
    }
}
