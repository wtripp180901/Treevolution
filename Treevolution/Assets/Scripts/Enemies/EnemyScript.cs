using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyScript : MonoBehaviour
{
    public Rigidbody enemyRigidbody;
    private List<Vector3> path;
    private Vector3 lastPosition;
    private int counter = 0;
    private bool moving = false;

    // Start is called before the first frame update
    void Start()
    {
        Pathfinder.UpdatePathfindingGraph();
        Vector3 treePosition = GameObject.FindGameObjectWithTag("Tree").transform.position;
        path = new List<Vector3>(Pathfinder.GetPath(transform.position, treePosition));
        lastPosition = gameObject.transform.position;
    }


    // Update is called once per physics frame
    void FixedUpdate()
    {
        if (counter < path.Count && !moving && (path[counter] - gameObject.transform.position).magnitude > 0.01f)
        {
            enemyRigidbody.MovePosition(gameObject.transform.position + (path[counter] - gameObject.transform.position) * Time.deltaTime);
            Debug.Log("Moving to: " + path[counter].ToString());
        }
        else if ((counter < path.Count) && (path[counter] - enemyRigidbody.position).magnitude <= 0.01f)
        {
            lastPosition = path[counter];
            counter+=1;
            Debug.Log(counter);
            moving = false;
        }
    }

    public void Damage()
    {
        Debug.Log("Enemy hit");
    }
}
