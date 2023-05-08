using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Timeline.Actions;
using UnityEngine;
using BuddyActions;
using Towers;


/// <summary>
/// Controls buddy behaviour
/// </summary>
public class BuddyScript : MonoBehaviour
{
    /// <summary>
    /// The list of instructions to be sequentially performed
    /// </summary>
    Queue<BuddyAction> actionQueue = new Queue<BuddyAction>();
    Rigidbody rig; //This is what physics are applied to in order to move the object

    /// <summary>
    /// If true means the buddy is ready to move to the next instruction
    /// </summary>
    bool isok = true;

    Vector3 directionVector;
    [SerializeField] float speed = 0.05f;
    [SerializeField] int damage = 2;

    [SerializeField] private float AttackRate = 1;
    float attackCooldown;

    [SerializeField] private float RepairRate = 1;
    float repairCooldown;

    public GameObject rangeIndicator;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();
        attackCooldown = AttackRate;
        repairCooldown = RepairRate;
        rangeIndicator?.SetActive(false);
    }

    public void SetupForTest(float speed)
    {
        rig = GetComponent<Rigidbody>();
        this.speed = speed;
    }

    BUDDY_ACTION_TYPES currentAction;

    Queue<GameObject> targets;
    GameObject currentTarget = null;

    /*Vector3[] pos = null;
    int Currentpos = -1;*/
    Vector3 moveTarget;

    // Update is called once per frame
    void Update()
    {
        //TODO: If actionQueue is not empty and you are not currently performing an action, start performing the next action
        //There is only one type of action impemented so far. If the current BuddyAction type is Move, start moving to the location in the BuddyAction
        //You can get a path to the location by calling Pathfinding.Pathfinder.GetPath(transform.position,location) (store this somewhere). This gives an
        //ordered list of points you move to in order to get to the location (this avoids walls).
        if (actionQueue.Count != 0 && isok)
        {
            BuddyAction temp = actionQueue.Dequeue();
            currentAction = temp.actionType;

            //StartCoroutine(Delay(2f, () =>
            //{

            //}));
            isok = false;
            switch (temp.actionType)
            {
                case BUDDY_ACTION_TYPES.Move:
                    //getpath
                    moveTarget = ((MoveBuddyAction)temp).location;
                    directionVector = getNewDirectionVector(moveTarget);
                    break;
                case BUDDY_ACTION_TYPES.Attack:
                case BUDDY_ACTION_TYPES.Repair:
                case BUDDY_ACTION_TYPES.Buff:
                    targets = new Queue<GameObject>(((TargetedBuddyAction)temp).targets);
                    if (targets.Count > 0) currentTarget = targets.Dequeue();
                    else isok = true;
                    break;
                case BUDDY_ACTION_TYPES.Defend:
                    rangeIndicator?.SetActive(true);
                    currentTarget = findNearest(GameObject.FindGameObjectWithTag("Logic").GetComponent<EnemyManager>().enemies);
                    break;
            }
        }
    }

    /// <summary>
    /// Returns direction vector to next position
    /// </summary>
    /// <param name="nextPosition">Target position to move to</param>
    /// <returns>Direction vector</returns>
    Vector3 getNewDirectionVector(Vector3 nextPosition)
    {
        Vector3 dirVec = (new Vector3(nextPosition.x, 0, nextPosition.z) - new Vector3(transform.position.x,0,transform.position.z)).normalized * speed;
        return dirVec;
    }

    //Used for physics updates to move the object
    private void FixedUpdate()
    {
        if (!isok)
        {
            switch (currentAction)
            {
                case BUDDY_ACTION_TYPES.Move:
                    movementFixedUpdate();
                    break;
                case BUDDY_ACTION_TYPES.Attack:
                    attackFixedUpdate();
                    break;
                case BUDDY_ACTION_TYPES.Repair:
                    repairFixedUpdate();
                    break;
                case BUDDY_ACTION_TYPES.Defend:
                    defendFixedUpdate();
                    break;
                case BUDDY_ACTION_TYPES.Buff:
                    buffFixedUpdate();
                    break;
                default:
                    break;
            }

        }
    }

    /// <summary>
    /// Attacks enemies within the buddy's range at a location
    /// </summary>
    void defendFixedUpdate()
    {
        if (currentTarget == null)
        {
            //Find all the entities in the Logic tag with the GameObject.FindWithTag function
            GameObject[] enemiesList = GameObject.FindGameObjectWithTag("Logic").GetComponent<EnemyManager>().enemies;
            //List<GameObject> enemiesList = GameObject.FindWithTag(" Logic ").GetComponent().enemies;
            //According to the nearest rule, find the enemy closest to the current character
            if (enemiesList.Length > 0)
            {
                currentTarget = findNearest(enemiesList);
            }
        }
        else
        {
            if (attackCooldown <= 0)
            {
                currentTarget.GetComponent<EnemyScript>().Damage(damage);
                attackCooldown = AttackRate;
            }
            else attackCooldown -= Time.fixedDeltaTime;
        }
    }

    /// <summary>
    /// Moves to target plants and activates EnterBuddyMode
    /// </summary>
    void buffFixedUpdate()
    {
        if (moveToTargetGameObject(currentTarget))
        {
            TowerScript tower = currentTarget.GetComponent<TowerScript>();
            tower?.EnterBuddyMode();
            if (targets.Count > 0) currentTarget = targets.Dequeue();
            else isok = true;
        }
    }


    /// <summary>
    /// Moves to walls and repairs them
    /// </summary>
    void repairFixedUpdate()
    {
        if (moveToTargetGameObject(currentTarget))
        {
            if (repairCooldown <= 0)
            {
                Debug.Log("repairing");
                currentTarget.GetComponent<WallScript>().Repair(4);
                repairCooldown = RepairRate;
                if (!currentTarget.GetComponent<WallScript>().isDestroyed)
                {
                    if (targets.Count > 0) currentTarget = targets.Dequeue();
                    else isok = true;
                }
            }
            else
            {
                repairCooldown -= Time.fixedDeltaTime;
            }
        }
    }


    /// <summary>
    /// Moves to enemies and attacks them
    /// </summary>
    void attackFixedUpdate()
    {
        if (currentTarget == null)
        {
            if (targets.Count > 0) currentTarget = targets.Dequeue();
            else isok = true;
        }
        else
        {
            if (moveToTargetGameObject(currentTarget))
            {
                if (attackCooldown <= 0)
                {
                    currentTarget.GetComponent<EnemyScript>().Damage(damage);
                    attackCooldown = AttackRate;
                }
                else attackCooldown -= Time.fixedDeltaTime;
            }
        }
    }

    /// <summary>
    /// Moves in the direction of directionVector until the targetted position is reached
    /// </summary>
    void movementFixedUpdate()
    {
        if (directionVector != null)
        {
            rig.MovePosition(transform.position + directionVector * Time.fixedDeltaTime);
            if (Vector3.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(moveTarget.x, moveTarget.z)) < 0.05f)
            {
                isok = true;
            }
        }
    }

    /// <summary>
    /// Moves towards a targetted GameObject
    /// </summary>
    /// <param name="target">The GameObject to move towards</param>
    /// <returns>Returns true if the target has been reached and false otherwise</returns>
    bool moveToTargetGameObject(GameObject target)
    {
        Vector3 vecToTarget = getNewDirectionVector(target.transform.position);
        if ((new Vector2(transform.position.x,transform.position.z) - new Vector2(target.transform.position.x,target.transform.position.z)).magnitude < 0.05f)
        {
            return true;
        }
        else
        {
            rig.MovePosition(transform.position + vecToTarget * Time.fixedDeltaTime);
            return false;
        }
    }

    /// <summary>
    /// Clears actionQueue and fills it with new instructions
    /// </summary>
    /// <param name="actions">New instructions</param>
    public void GiveInstructions(List<BuddyAction> actions)
    {
        actionQueue.Clear();
        targets?.Clear();
        rangeIndicator?.SetActive(false);
        isok = true;
        //Add actions to actionQueue
        foreach (BuddyAction item in actions)
        {
            actionQueue.Enqueue(item);
        }
    }

    /// <summary>
    /// Finds the closest enemy within range
    /// </summary>
    /// <param name="enemiesList">List of active enemies in the scene</param>
    /// <returns>Returns closest enemy is one exists and is in range, returns null otherwise</returns>
    GameObject findNearest(GameObject[] enemiesList){
        float dis1 = 9999;//Its own attack range, this can be modified, if the enemy is larger than this range then there is no target to attack
        GameObject target = null;
        foreach(GameObject obj in enemiesList){
            float dis2 = Vector3.Distance(obj.transform.position, transform.position);
            if(dis2<dis1 && (new Vector2(transform.position.x,transform.position.z) - new Vector2(obj.transform.position.x,obj.transform.position.z)).magnitude < 0.25f){
                target = obj;
                dis1 = dis2;
            }
        }
        return target;
    }

    public void GetTestData(out Queue<GameObject> targets,out GameObject currentTarget,out bool isOk)
    {
        targets = this.targets;
        currentTarget = this.currentTarget;
        isOk = isok;
    }

    public void GetTestData(out Queue<BuddyAction> actionQueue)
    {
        actionQueue = this.actionQueue;
    }
}
