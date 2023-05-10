using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls behaviour of walls
/// </summary>
public class WallScript : MonoBehaviour, IRuntimeMovableBehaviourScript
{
    /// <summary>
    /// The starting health of the wall
    /// </summary>
    [SerializeField] int baseHealth = 10;
    int health;

    Pathfinding.PathfindingObstacle nodeGenerator;
    bool _isDestroyed = false;
    /// <summary>
    /// Returns true if the wall is currently destroyed
    /// </summary>
    public bool isDestroyed { get { return _isDestroyed; } }
    /// <summary>
    /// Returns true if the wall isn't at full health
    /// </summary>
    public bool isDamaged { get { return health < baseHealth; } }


    Collider myCollider;

    float baseScale;

    /// <summary>
    /// The material to be applied to the object indicating if the wall is currently being penalised for movement when it is
    /// </summary>
    [SerializeField]
    Material enabled;
    /// <summary>
    /// The material to be applied to the object indicating if the wall is currently being penalised for movement when it is not
    /// </summary>
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

    /// <summary>
    /// Damage the wall and destroy it if its health drops below zero
    /// </summary>
    /// <param name="power">The damage to the wall</param>
    public void Damage(int power)
    {
        health -= power;
        if (health <= 0)
        {
            health = 0;
            SetDestroyed(true);
        }
    }

    /// <summary>
    /// Restores the walls health and reactivates it if its health reaches max again
    /// </summary>
    /// <param name="repaired">The amount to repair</param>
    public void Repair(int repaired)
    {
        health += repaired;
        if (health >= baseHealth)
        {
            health = baseHealth;
            SetDestroyed(false);
        }
    }

    /// <summary>
    /// Sets whether the wall is currently destroyed and should block enemies
    /// </summary>
    /// <param name="destroyed"></param>
    void SetDestroyed(bool destroyed)
    {
        myCollider.enabled = !destroyed;
        nodeGenerator.SetSendsNodes(!destroyed);
        _isDestroyed = destroyed;

        float scale = baseScale;
        if (destroyed) scale *= 0.05f;
        transform.localScale = new Vector3(transform.localScale.x, scale, transform.localScale.z);
    }

    public void ApplyMovementPenalty()
    {
        myCollider.enabled = false;
        if(gameObject.transform.childCount > 0) gameObject.transform.GetChild(gameObject.transform.childCount - 1).GetComponent<Renderer>().material = disabled;
    }

    public void EndMovementPenalty()
    {
        if (!isDestroyed) myCollider.enabled = true;
        if (gameObject.transform.childCount > 0) gameObject.transform.GetChild(gameObject.transform.childCount - 1).GetComponent<Renderer>().material = enabled;
    }

    public void SetupForTest(Collider collider, Pathfinding.PathfindingObstacle nodeGenerator)
    {
        myCollider = collider;
        this.nodeGenerator = nodeGenerator;
    }

}

