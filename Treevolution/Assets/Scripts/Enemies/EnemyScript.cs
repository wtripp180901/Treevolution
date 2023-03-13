using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using TMPro;

public class EnemyScript : MonoBehaviour
{
    public Rigidbody rig;
    [SerializeField] // This allows the private field to be edited from the Unity inspecter
    private float speed = 0.1f;

    Vector3[] path;
    bool followingPath = true;
    Vector3 directionVector;
    Vector3 currentTarget;
    int pathCounter = 0;
    int health = 10;

    [SerializeField]
    private List<string> climbableTags = new List<string>() { "Wall", "Tower" };
    bool climbing = false;
    float targetHeight;
    public float baseHeight;
    public TMP_Text debugText;

    [SerializeField]
    private AudioSource damageAudio;
    [SerializeField]
    private AudioSource spawnAudio;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();
        Initialise();
        spawnAudio.Play();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 pos = transform.position;
        if (followingPath && rig.velocity.y >= -5f)
        {
            Vector3 enemyToTarget = currentTarget - pos;
            enemyToTarget.y = 0;
            if (enemyToTarget.magnitude < 0.005f)
            {
                startMoveToNextTarget();
            }
            directionVector = enemyToTarget.normalized * speed;
            rig.MovePosition(pos + directionVector);
        }
        if (climbing)
        {
            if (pos.y < targetHeight)
            {
                rig.MovePosition(pos + transform.up * speed);
            }
            else
            {
                rig.MovePosition(pos + new Vector3(60 * directionVector.x, 0, 60 * directionVector.z));
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
            directionVector.y = 0;
            transform.rotation = Quaternion.LookRotation(directionVector, transform.up);
            pathCounter += 1;
        }
    }

    bool hasHitFloor = false;
    private void OnCollisionEnter(Collision collision)
    {
        Collider otherCollider = collision.collider;
        if (otherCollider.gameObject.tag == "Tree")
        {
            Debug.Log("Reached tree");
            followingPath = false;
        }
        else if (otherCollider.gameObject.tag == "Bullet")
        {
            Damage();
        }
        else if (climbableTags.Contains(otherCollider.gameObject.tag))
        {
            float topOfCollider = otherCollider.bounds.extents.y / 2 + collision.gameObject.transform.position.y;
            float heightAboveObject = topOfCollider + gameObject.GetComponent<Collider>().bounds.extents.y;// + 0.01f;
            if (transform.position.y < topOfCollider)
            {
                followingPath = false;
                rig.useGravity = false;
                climbing = true;
                targetHeight = heightAboveObject;
            }
        }
        if (!hasHitFloor && (GetComponent<Collider>().gameObject.tag == "Floor" || GetComponent<Collider>().gameObject.tag == "Wall"))
        {
            hasHitFloor = true;
        }
    }

    private void Initialise()
    {
        Vector3 pos = transform.position;
        path = Pathfinding.Pathfinder.GetPath(pos, GameObject.FindGameObjectWithTag("Tree").transform.position);
        baseHeight = pos.y;
        rig.freezeRotation = true;
        startMoveToNextTarget();
    }

    IEnumerator DamageIndicator()
    {
        Color defaultColour = GetComponent<Renderer>().material.color;
        damageAudio.Play();
        GetComponent<Renderer>().material.color = Color.red;
        yield return new WaitForSeconds(0.3f);
        GetComponent<Renderer>().material.color = defaultColour;
    }

    public void Damage()
    {
        health -= 1;
        if (health <= 0)
        {
            DestroyEnemy();
        }
        else StartCoroutine(DamageIndicator());
    }

    public void DestroyEnemy()
    {
        GameObject.FindWithTag("Logic").GetComponent<EnemyManager>().RemoveEnemy(gameObject);
        Destroy(gameObject);
    }
}