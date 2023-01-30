using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyScript : MonoBehaviour
{
    public Rigidbody enemyRigidbody;
    public float speedMultiplier = 1.0f;
    private List<Vector3> path;
    private int counter = 0;

    private void followPath()
    {
        Vector3 vectorToNode = path[counter] - gameObject.transform.position;
        if (counter < path.Count && (vectorToNode).magnitude > 0.01f)
        {
            //transform.LookAt(path[counter]);
            enemyRigidbody.MovePosition(gameObject.transform.position + (vectorToNode).normalized * Time.fixedDeltaTime);
            Debug.Log("Moving to: " + path[counter].ToString());
        }
        else if ((counter < path.Count) && (vectorToNode).magnitude <= 0.01f)
        {
            counter += 1;
            Debug.Log(counter);
        }
    }

    private void moveToTree()
    {
        Vector3 vectorToTree = GameObject.FindGameObjectWithTag("Tree").transform.position - gameObject.transform.position;
        vectorToTree.y = 0; // Stay on same y-level
        //transform.LookAt(GameObject.FindGameObjectWithTag("Tree").transform.position);
        enemyRigidbody.MovePosition(gameObject.transform.position + vectorToTree.normalized * Time.fixedDeltaTime);
        Debug.Log("Moving To Tree");
    }

    private void moveInDirection(Vector3 direction)
    {
        direction.y = 0; // Stay on same y-level
        enemyRigidbody.MovePosition(gameObject.transform.position + direction.normalized * Time.fixedDeltaTime);
        Debug.Log("Moving In Direction " + direction.ToString());
    }


    // Start is called before the first frame update
    void Start()
    {
        Pathfinder.UpdatePathfindingGraph();
        Vector3 treePosition = GameObject.FindGameObjectWithTag("Tree").transform.position;
        path = new List<Vector3>(Pathfinder.GetPath(transform.position, treePosition));
    }


    // Update is called once per physics frame
    void FixedUpdate()
    {
        moveToTree();
    }

    private void OnTriggerStay(Collider other)
    {
        moveInDirection(Vector3.back * 2f); // If the enemy collides with an object then it will start moving in specified direction until it is no longer colliding
    }


    public void Damage()
    {
        Debug.Log("Enemy hit");
    }
}
