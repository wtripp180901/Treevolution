using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallScript : MonoBehaviour, IRuntimeMovableBehaviourScript
{
    [SerializeField] int baseHealth = 10;
    int health;

    Pathfinding.PathfindingObstacle nodeGenerator;
    bool isDestroyed = false;

    Collider myCollider;

    float baseScale;

    [SerializeField]
    Material enabled;
    [SerializeField]
    Material disabled; 
    
    // Start is called before the first frame update
    void Start()
    {
        nodeGenerator = GetComponent<Pathfinding.PathfindingObstacle>();
        myCollider = GetComponent<Collider>();
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
        myCollider.enabled = !destroyed;
        nodeGenerator.SetSendsNodes(!destroyed);
        isDestroyed = destroyed;

        float scale = baseScale;
        if (destroyed) scale *= 0.05f;
        transform.localScale = new Vector3(transform.localScale.x, scale, transform.localScale.z);
    }

    public void ApplyMovementPenalty()
    {
        myCollider.enabled = false;
        gameObject.transform.GetChild(gameObject.transform.childCount-1).GetComponent<Renderer>().material = disabled;
    }

    public void EndMovementPenalty()
    {
        if (!isDestroyed) myCollider.enabled = true;
        gameObject.transform.GetChild(gameObject.transform.childCount - 1).GetComponent<Renderer>().material = enabled;
    }
}
