using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

using TMPro;

public class EnemyScript : MonoBehaviour
{
    RoundTimer roundTimer;
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
    float inWallDamageInterval = 0.5f;
    float currentInWallInterval = 0f;
    bool inWall = false;

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
        roundTimer = GameObject.Find("Logic").GetComponent<RoundTimer>();
    }

    private void Update()
    {
        if(inWall)
        {
            currentInWallInterval -= Time.deltaTime;
            if(currentInWallInterval < 0f)
            {
                currentInWallInterval = inWallDamageInterval;
                Damage(1);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (roundTimer.IsPaused)
            return;
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
            Damage(1);
        }
        if (!hasHitFloor && (GetComponent<Collider>().gameObject.tag == "Floor" || GetComponent<Collider>().gameObject.tag == "Wall"))
        {
            hasHitFloor = true;
        }
    }

    private void OnTriggerEnter(Collider trigger)
    {
        inWall = true;
    }

    private void OnTriggerExit(Collider trigger)
    {
        inWall = false;
        currentInWallInterval = inWallDamageInterval;
    }

    private void Initialise()
    {
        Vector3 pos = transform.position;
        path = Pathfinding.Pathfinder.GetPath(pos, GameObject.FindGameObjectWithTag("Tree").transform.position);
        rig.freezeRotation = true;
        startMoveToNextTarget();

        Vector2 screenSpawnPos = Camera.main.WorldToScreenPoint(transform.position);
        SpawnDirectionIndicator spawnDirectionIndicator = null;
        if (screenSpawnPos.x < 0) spawnDirectionIndicator = GameObject.FindWithTag("LeftIndicator").GetComponent<SpawnDirectionIndicator>();
        if (screenSpawnPos.x > Screen.width) spawnDirectionIndicator = GameObject.FindWithTag("RightIndicator").GetComponent<SpawnDirectionIndicator>();
        if(spawnDirectionIndicator != null) spawnDirectionIndicator.IndicateDirection();
    }

    IEnumerator DamageIndicator()
    {
        List<Color> defaultColours = new List<Color>();
        Renderer[] childRenderers = transform.GetComponentsInChildren<Renderer>();
        for(int i = 0;i < childRenderers.Length; i++)
        {
            defaultColours.Add(childRenderers[i].material.color);
            childRenderers[i].material.color = Color.red;
        }
        damageAudio.Play();
        yield return new WaitForSeconds(0.3f);
        for (int i = 0; i < defaultColours.Count; i++)
        {
            childRenderers[i].material.color = defaultColours[i];
        }
    }

    public void Damage(int power)
    {
        health -= power;
        if (health <= 0)
        {
            DestroyEnemy(true);
        }
        else StartCoroutine(DamageIndicator());
    }

    public void DestroyEnemy(bool killedByPlayer)
    {
        GameObject.FindWithTag("Logic").GetComponent<EnemyManager>().RemoveEnemy(gameObject, killedByPlayer);
        Destroy(gameObject);
    }
}