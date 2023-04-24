using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Controls enemy behaviour.
/// </summary>
public class EnemyScript : MonoBehaviour
{
    /// <summary>
    /// Health given to the current enemy (default = 10).
    /// </summary>
    public int health = 10;
    /// <summary>
    /// Determines if the current enemy is flying or not (off by default).
    /// </summary>
    public bool flying = false;
    /// <summary>
    /// Sets the speed of the current enemy.
    /// </summary>
    [SerializeField] // This allows the private field to be edited from the Unity inspecter
    private float speed = 0.01f;

    Vector3[] path;
    bool followingPath = true;
    Vector3 directionVector;
    Vector3 currentTarget;
    int pathCounter = 0;

    [SerializeField]
    float inWallDamageInterval = 0.5f;
    float currentInWallInterval = 0f;
    bool inWall = false;

    
    /// <summary>
    /// Sound effect to be played when the enemy gets damaged.
    /// </summary>
    [SerializeField]
    private AudioSource damageAudio;
    /// <summary>
    /// Sound effect to be played when the enemy dies.
    /// </summary>
    [SerializeField]
    private AudioSource deathAudio;

    /// <summary>
    /// Current Enemy's Rigidbody component.
    /// </summary>
    private Rigidbody _rigidbody;
    /// <summary>
    /// Current Enemy's Healthbar component.
    /// </summary>
    private HealthBar _healthBar;
    /// <summary>
    /// Pathfinding path to follow.
    /// </summary>
    private Vector3[] _path;
    /// <summary>
    /// Whether the current enemy is following a path.
    /// </summary>
    private bool _followingPath = true;
    /// <summary>
    /// Current direction of movement.
    /// </summary>
    private Vector3 _directionVector;
    /// <summary>
    /// Current target node that the enemy is moving towards.
    /// </summary>
    private Vector3 _currentTarget;
    /// <summary>
    /// Counter for the pathfinding path node, specifying which node the current enemy is moving towards.
    /// </summary>
    private int _pathCounter = 0;
    /// <summary>
    /// Running RoundTimer instance.
    /// </summary>
    private RoundTimer _roundTimer;
    /// <summary>
    /// Running EnemyManager instance.
    /// </summary>
    private EnemyManager _enemyManager;
    /// <summary>
    /// Reference to the Left Spawn Indicator.
    /// </summary>
    private SpawnDirectionIndicator _leftIndicator;
    /// <summary>
    /// Reference to the Right Spawn Indicator.
    /// </summary>
    private SpawnDirectionIndicator _rightIndicator;

    /// <summary>
    /// Initial orientation of the enemy prefab
    /// </summary>
    private Vector3 _defaultOrientation;

    /// <summary>
    /// Determines if enemy requests path around walls or not
    /// </summary>
    public bool AvoidsWall = true;

    /// <summary>
    /// Determines if enemies destroy walls on collision
    /// </summary>
    public bool DamagesWall = false;

    /// <summary>
    /// Start runs when loading the GameObject that this script is attached to.
    /// </summary>
    void Start()
    {
        _defaultOrientation = transform.rotation.eulerAngles;
        _roundTimer = GameObject.Find("Logic").GetComponent<RoundTimer>();
        _rigidbody = GetComponent<Rigidbody>();
        _healthBar = GetComponent<HealthBar>();
        _leftIndicator = GameObject.FindWithTag("LeftIndicator").GetComponent<SpawnDirectionIndicator>();
        _rightIndicator = GameObject.FindWithTag("RightIndicator").GetComponent<SpawnDirectionIndicator>();
        _enemyManager = GameObject.FindWithTag("Logic").GetComponent<EnemyManager>();
        if (flying)
            _rigidbody.useGravity = false;
        else
            _rigidbody.useGravity = true;
        Pathfinding.PathfindingUpdatePublisher.RefindPathNeededEvent.AddListener(restartPathfinding);
        Initialise();
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

    /// <summary>
    /// Called once per fixed framerate frame. This moves the enemy towards its current pathfinding target.
    /// </summary>
    void FixedUpdate()
    {
        if (_roundTimer != null && _roundTimer.isPaused)
            return;
        Vector3 pos = transform.position;
        if (_followingPath && _rigidbody.velocity.y >= -5f)
        {
            Vector3 enemyToTarget = _currentTarget - pos;
            enemyToTarget.y = 0;
            if (enemyToTarget.magnitude < 0.005f)
            {
                StartMoveToNextTarget();
            }
            _directionVector = enemyToTarget.normalized * speed;
            _rigidbody.MovePosition(pos + _directionVector);
        }
    }

    private void restartPathfinding()
    {
        _pathCounter = 0;
        _path = Pathfinder.GetPath(transform.position, GameObject.FindGameObjectWithTag("Tree").transform.position, AvoidsWall);
        StartMoveToNextTarget();
    }

    /// <summary>
    /// Starts moving the enemy to the next pathfinding target.
    /// </summary>
    private void StartMoveToNextTarget()
    {
        if (_pathCounter >= _path.Length)
        {
            _followingPath = false;
        }
        else
        {
            _currentTarget = _path[_pathCounter];
            _directionVector = (_currentTarget - transform.position).normalized * speed;
            _directionVector.y = 0;
            transform.rotation = Quaternion.Euler(Quaternion.LookRotation(_directionVector, transform.up).eulerAngles + _defaultOrientation);
            _pathCounter += 1;
        }
        //GameObject.FindGameObjectWithTag("DebugText").GetComponent<TMP_Text>().text = "pathLen=" + _path.Length.ToString() + "\nfollowingPath=" + _followingPath.ToString() + "\vtarg=" + _currentTarget.ToString() + "\npathCounter=" + _pathCounter.ToString() + "\nTimer=" + _roundTimer.isRunning;
    }

    /// <summary>
    /// Runs when the enemy collides with another objects collider, such as reaching the Home Tree or being hit by a projectile.
    /// </summary>
    /// <param name="collision">Collision data.</param>
    private void OnCollisionEnter(Collision collision)
    {
        Collider otherCollider = collision.collider;
        if (otherCollider.gameObject.tag == "Tree")
        {
            Debug.Log("Reached tree");
            _followingPath = false;
        }
        else if (collision.gameObject.tag == "Bullet")
        {
            Damage(collision.gameObject.GetComponent<BulletScript>().damage);
        }
    }

    private void OnTriggerEnter(Collider trigger)
    {
        GameObject collision = trigger.gameObject;
        if (DamagesWall && collision.tag == "Wall")
        {
            collision.GetComponent<WallScript>().Damage(10);
        }
        else
        {
            inWall = true;
        }
    }

    private void OnTriggerExit(Collider trigger)
    {
        if (trigger.gameObject.tag == "Wall")
        {
            inWall = false;
            currentInWallInterval = inWallDamageInterval;
        }
    }

    /// <summary>
    /// Initialises some of the enemies initial properties, such as healthbar, rotation, and spawn indicator.
    /// </summary>
    private void Initialise()
    {
        _healthBar.SetMaxHealth(health);
        if (flying)
        {
            transform.position = transform.position + Vector3.up * 0.25f;
        }
        Vector3 pos = transform.position;
        _path = Pathfinder.GetPath(pos, GameObject.FindGameObjectWithTag("Tree").transform.position,AvoidsWall);
        _rigidbody.freezeRotation = true;

        StartMoveToNextTarget();

        SpawnIndicator();
    }

    /// <summary>
    /// Indicates the spawn direction of the enemy if it is out of the player's fielf of view.
    /// </summary>
    private void SpawnIndicator()
    {
        Camera cam = Camera.main;// CameraCache.Main;
        Vector3 viewPortSpawnPos = cam.WorldToViewportPoint(transform.position);
        SpawnDirectionIndicator spawnDirectionIndicator = null;
        if (viewPortSpawnPos.x < 0f || (viewPortSpawnPos.z < 0 && viewPortSpawnPos.x < 0.5f)) spawnDirectionIndicator = _leftIndicator;
        if (viewPortSpawnPos.x > 1f || (viewPortSpawnPos.z < 0 && viewPortSpawnPos.x >= 0.5f)) spawnDirectionIndicator = _rightIndicator;
        if (spawnDirectionIndicator != null) spawnDirectionIndicator.IndicateDirection();
    }

    /// <summary>
    /// Indicates the enemy has been damaged by turning its body red for a brief time and playing the damaged sound effect.
    /// </summary>
    /// <returns>This method runs a coroutine and so a <c>yield return</c> is used.</returns>
    private IEnumerator DamageIndicator()
    {
        List<Color> defaultColours = new List<Color>();
        Renderer[] childRenderers = transform.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < childRenderers.Length; i++)
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

    /// <summary>
    /// Kills the current enemy, playing a death sound and squashing the enemies geometry.
    /// </summary>
    /// <returns>This method runs a coroutine and so a <c>yield return</c> is used.</returns>
    private IEnumerator KillEnemy()
    {
        _followingPath = false;
        gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x, 0, gameObject.transform.localScale.z);
        deathAudio.Play();
        yield return new WaitForSeconds(1f);
        DestroyEnemy(true);
    }

    /// <summary>
    /// Damages the current enemy, and checks if it has run out of health.
    /// </summary>
    /// <param name="power">Power to damage the enemy by.</param>
    public void Damage(int power)
    {
        try
        {
            health -= power;
            _healthBar.SetHealth(health);
            if (health <= 0)
            {
                StartCoroutine(KillEnemy());
            }
            else StartCoroutine(DamageIndicator());
        }
        catch { }
    }

    /// <summary>
    /// Destroys the enemy object and removes it from the EnemyManager's enemy list.
    /// </summary>
    /// <param name="killedByPlayer">Whether the enemy was killed by the player, or is bein destroyed as part of the ClearEnemies method and so shouldn't count towards the user's score.</param>
    public void DestroyEnemy(bool killedByPlayer)
    {
        Pathfinding.PathfindingUpdatePublisher.RefindPathNeededEvent.RemoveListener(restartPathfinding);
        _enemyManager.RemoveEnemy(gameObject, killedByPlayer);
        Destroy(gameObject.GetComponent<Collider>().gameObject);
    }
}