using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Timeline.Actions;
using UnityEngine;

public class BuddyScript : MonoBehaviour
{
    Queue<BuddyAction> actionQueue = new Queue<BuddyAction>();
    Rigidbody rig; //This is what physics are applied to in order to move the object

    bool isok = true;

    Vector3 directionVector;
    [SerializeField] float speed = 0.05f;
    [SerializeField] int damage = 2;

    [SerializeField] private float AttackRate = 1;
    float attackCooldown;

    [SerializeField] private float RepairRate = 1;
    float repairCooldown;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();
        attackCooldown = AttackRate;
        repairCooldown = RepairRate;
    }

    BUDDY_ACTION_TYPES currentAction;

    Queue<GameObject> targets;
    GameObject currentTarget = null;

    Vector3[] pos=null;
    int Currentpos=-1;

    GameObject[] enemiesList = null;

    // Update is called once per frame
    void Update()
    {
        //TODO: If actionQueue is not empty and you are not currently performing an action, start performing the next action
        //There is only one type of action impemented so far. If the current BuddyAction type is Move, start moving to the location in the BuddyAction
        //You can get a path to the location by calling Pathfinding.Pathfinder.GetPath(transform.position,location) (store this somewhere). This gives an
        //ordered list of points you move to in order to get to the location (this avoids walls).
        if (actionQueue.Count != 0 && isok)
        {
            BuddyAction temp= actionQueue.Dequeue();
            currentAction = temp.actionType;

            //StartCoroutine(Delay(2f, () =>
            //{

            //}));
            isok = false;
            switch (temp.actionType)
            {
                case BUDDY_ACTION_TYPES.Move:
                    //getpath
                    pos = Pathfinding.Pathfinder.GetPath(transform.position, ((MoveBuddyAction)temp).location, false);
                    Currentpos = 0;
                    if (pos != null && pos.Length > 0) directionVector = getNewDirectionVector(pos[0]);
                    break;
                case BUDDY_ACTION_TYPES.Attack:
                case BUDDY_ACTION_TYPES.Repair:
                    targets = new Queue<GameObject>(((TargetedBuddyAction)temp).targets);
                    if (targets.Count > 0) currentTarget = targets.Dequeue();
                    else isok = true;
                    break;
                case BUDDY_ACTION_TYPES.Defend:
                    isok = false;
                    //Find all the entities in the Logic tag with the GameObject.FindWithTag function
                    enemiesList = GameObject.FindGameObjectWithTag("Logic").GetComponent<EnemyManager>().enemies;
                    //List<GameObject> enemiesList = GameObject.FindWithTag(" Logic ").GetComponent().enemies;
                    //According to the nearest rule, find the enemy closest to the current character
                    if (enemiesList.Length > 0)
                    {
                        GameObject target = findNearest(enemiesList);
                        if (target != null)
                        {
                            target.GetComponent<EnemyScript>().Damage(2);
                        }
                    }
                    break;
            }

        }
    }

    IEnumerator Delay(float v, Action value)
    {
        yield return new WaitForSeconds(v);
        value?.Invoke();
    }

    Vector3 getNewDirectionVector(Vector3 nextPosition)
    {
        Vector3 dirVec = (new Vector3(nextPosition.x,transform.position.y,nextPosition.z) - transform.position).normalized * speed;
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
                default:
                    break;
            }
            
        }
    }

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

    void movementFixedUpdate()
    {
        if (pos != null)
        {
            rig.MovePosition(transform.position + directionVector * Time.fixedDeltaTime);
            if (Vector3.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(pos[Currentpos].x, pos[Currentpos].z)) < 0.05f)
            {
                Currentpos++;
                if (Currentpos >= pos.Length)
                {
                    isok = true;
                    Currentpos = -1;
                    pos = null;
                }
                else
                {
                    directionVector = getNewDirectionVector(pos[Currentpos]);
                }
            }
        }
    }

    bool moveToTargetGameObject(GameObject target)
    {
        Vector3 vecToTarget = new Vector3(target.transform.position.x, 0, target.transform.position.z) - new Vector3(transform.position.x, 0, transform.position.z);
        if (vecToTarget.magnitude < 0.05f)
        {
            return true;
        }
        else
        {
            rig.MovePosition(transform.position + vecToTarget.normalized * speed);
            return false;
        }
    }

    //As a test I am calling this from PhaseTransition.cs, currently 2 Move actions are added. When fully working, the buddy will move aroud the wall and
    //then back to its original location - Will
    public void GiveInstructions(List<BuddyAction> actions)
    {
        //Add actions to actionQueue
        foreach (BuddyAction item in actions)
        {
            actionQueue.Enqueue(item);
        }
    }

    GameObject findNearest(GameObject[] enemiesList){
        float dis1 = 9999;//Its own attack range, this can be modified, if the enemy is larger than this range then there is no target to attack
        GameObject target = null;
        foreach(GameObject obj in enemiesList){
            float dis2 = Vector3.Distance(obj.transform.position, transform.position);
            if(dis2<dis1){
                target = obj;
                dis1 = dis2;
            }
        }
        return target;
    }
}