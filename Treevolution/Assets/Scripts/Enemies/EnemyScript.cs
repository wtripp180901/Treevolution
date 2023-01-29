using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyScript : MonoBehaviour
{
    public Rigidbody enemyRigidbody;
    // Start is called before the first frame update
    void Start()
    {
        Pathfinder.UpdatePathfindingGraph();
        Vector3 treePosition = GameObject.FindGameObjectWithTag("Tree").transform.position;
        Vector3[] pathToTree = Pathfinder.GetPath(transform.position, treePosition);
        moveEnemy(pathToTree);
    }

    private void moveEnemy(Vector3[] path)
    {
        foreach (Vector3 node in path)
        {
            enemyRigidbody.MovePosition(node);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damage()
    {
        Debug.Log("Enemy hit");
    }
}
