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
    private HealthBar healthBar;
    private Vector3[] path;
    private bool followingPath = true;
    private Vector3 directionVector;
    private Vector3 currentTarget;
    private int pathCounter = 0;
    public int health = 10;
    public bool flying = false;
    [SerializeField]
    private AudioSource damageAudio;
    [SerializeField]
    private AudioSource spawnAudio;
    private EnemyManager enemyManager;
    private SpawnDirectionIndicator leftIndicator;
    private SpawnDirectionIndicator rightIndicator;

    private Vector3 defaultOrientation;

    // Start is called before the first frame update
    void Start()
    {
        defaultOrientation = transform.rotation.eulerAngles;
        roundTimer = GameObject.Find("Logic").GetComponent<RoundTimer>();
        rig = GetComponent<Rigidbody>();
        healthBar = GetComponent<HealthBar>();
        leftIndicator = GameObject.FindWithTag("LeftIndicator").GetComponent<SpawnDirectionIndicator>();
        rightIndicator = GameObject.FindWithTag("RightIndicator").GetComponent<SpawnDirectionIndicator>();
        enemyManager = GameObject.FindWithTag("Logic").GetComponent<EnemyManager>();
        if (flying)
            rig.useGravity = false;
        else
            rig.useGravity = true;
        Initialise();
        //spawnAudio.Play();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (roundTimer != null && roundTimer.IsPaused)
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
            transform.rotation = Quaternion.Euler(Quaternion.LookRotation(directionVector, transform.up).eulerAngles+defaultOrientation);
            pathCounter += 1;
        }
        GameObject.FindGameObjectWithTag("DebugText").GetComponent<TMP_Text>().text = "pathLen=" + path.Length.ToString() + "\nfollowingPath=" + followingPath.ToString() + "\vtarg=" + currentTarget.ToString() + "\npathCounter=" + pathCounter.ToString() + "\nTimer=" + roundTimer.isRunning;
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
        if (!hasHitFloor && (otherCollider.gameObject.tag == "Floor" || otherCollider.gameObject.tag == "Wall"))
        {
            hasHitFloor = true;
        }
    }

    private void Initialise()
    {
        healthBar.SetMaxHealth(health);
        if (flying)
        {
            transform.position = transform.position + Vector3.up * 0.25f;
        }
        Vector3 pos = transform.position;
        path = Pathfinder.GetPath(pos, GameObject.FindGameObjectWithTag("Tree").transform.position);
        rig.freezeRotation = true;

        startMoveToNextTarget();

        Vector2 screenSpawnPos = Camera.main.WorldToScreenPoint(transform.position);
        SpawnDirectionIndicator spawnDirectionIndicator = null;
        if (screenSpawnPos.x < 0) spawnDirectionIndicator = leftIndicator;
        if (screenSpawnPos.x > Screen.width) spawnDirectionIndicator = rightIndicator;
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
        healthBar.SetHealth(health);
        if (health <= 0)
        {
            DestroyEnemy(true);
        }
        else StartCoroutine(DamageIndicator());
    }

    public void DestroyEnemy(bool killedByPlayer)
    {
        
        enemyManager.RemoveEnemy(gameObject, killedByPlayer);
        Destroy(gameObject);
    }
}