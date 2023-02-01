using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Rigidbody rig;
    [SerializeField]
    private float speed = 0.1f;

    Vector3[] path;
    public bool followingPath = true;
    Vector3 directionVector;
    Vector3 currentTarget;
    int pathCounter = 0;

    [SerializeField]
    private List<string> climbableTages = new List<string>() { "Wall", "Tower" };
    public bool climbing = false;
    float targetHeight;
    float baseHeight;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();
        Vector3 pos = transform.position;
        path = Pathfinding.Pathfinder.GetPath(pos, GameObject.FindGameObjectWithTag("Tree").transform.position);
        baseHeight = pos.y;
        startMoveToNextTarget();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        if (followingPath)
        {
            if((currentTarget - pos).magnitude < 0.05f)
            {
                startMoveToNextTarget();
            }
            rig.MovePosition(pos + directionVector);
        }
        if (climbing)
        {
            if(pos.y < targetHeight)
            {
                rig.MovePosition(pos + transform.up * speed);
            }
            else
            {
                climbing = false;
                followingPath = true;
                rig.useGravity = true;
            }
        }
    }

    private void startMoveToNextTarget()
    {
        if (pathCounter >= path.Length)
        {
            followingPath = false;
        }
        else
        {
            currentTarget = path[pathCounter];
            directionVector = (currentTarget - transform.position).normalized * speed;
            pathCounter += 1;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Collider collider = collision.collider;
        if(collider.gameObject.tag == "Tree")
        {
            Debug.Log("Reached tree");
            followingPath = false;
        }
        else if(climbableTages.Contains(collider.gameObject.tag))
        {
            float topOfCollider = collider.bounds.extents.y + collider.gameObject.transform.position.y;
            float heightAboveObject = topOfCollider + GetComponent<Collider>().bounds.extents.y + 0.1f;
            if (transform.position.y < topOfCollider)
            {
                followingPath = false;
                rig.useGravity = false;
                climbing = true;
                targetHeight = heightAboveObject;
            }
        }
    }
}
