using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallScript : MonoBehaviour
{
    [SerializeField] int baseHealth = 10;
    int health;

    Pathfinding.PathfindingObstacle nodeGenerator;
    bool isDestroyed = false;

    float baseScale;
    
    // Start is called before the first frame update
    void Start()
    {
        nodeGenerator = GetComponent<Pathfinding.PathfindingObstacle>();
        health = baseHealth;
        baseScale = transform.localScale.y;
    }

    public void Damage(int power)
    {
        health -= power;
        if(health <= 0)
        {
            health = 0;
            SetDestroyed(true);
        }
    }

    public void Repair(int repaired)
    {
        health += repaired;
        if(repaired >= baseHealth)
        {
            health = baseHealth;
            SetDestroyed(false);
        }
    }

    void SetDestroyed(bool destroyed)
    {
        GetComponent<Collider>().enabled = !destroyed;
        nodeGenerator.SendNodes = !destroyed;
        isDestroyed = destroyed;

        float scale = baseScale;
        if (destroyed) scale *= 0.05f;
        transform.localScale = new Vector3(transform.localScale.x, scale, transform.localScale.z);

        Pathfinding.PathfindingUpdatePublisher.NotifyObstacleChanged();
    }
}
