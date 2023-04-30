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
    float speed = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody>();
    }
    Vector3[] pos=null;
    int Currentpos=-1;
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
            if (temp.actionType == BUDDY_ACTION_TYPES.Move)
            {
                isok = false;
                //getpath
                 pos=Pathfinding.Pathfinder.GetPath(transform.position,temp.location);
                Currentpos = 0;
                if (pos != null && pos.Length > 0) directionVector = getNewDirectionVector(pos[0]);
                //StartCoroutine(Delay(2f, () =>
                //{

                //}));
            else if (temp.actionType == BUDDY_ACTION_TYPES.BuddyAction)
            {
                isok = false;
                //Find all the entities in the Logic tag with the GameObject.FindWithTag function
                GameObject[] enemiesList = GameObject.FindGameObjectsWithTag(" Logic ");
                //List<GameObject> enemiesList = GameObject.FindWithTag(" Logic ").GetComponent().enemies;
                //According to the nearest rule, find the enemy closest to the current character
                if(enemiesList.Length>0){
                    GameObject target = findNearest(enemiesList);
                    if(target!=null){
                        target.GetComponent(EnemyScript).Damage();
                    }
                }
            }
            else if (temp.actionType == BUDA 0.5 reduction in fire cd equals an increase in tower attack speedDY_ACTION_TYPES.Buff)
            {
                isok = false;
                GameObject[] towerList = GameObject.FindGameObjectsWithTag("tower");
                if(towerList.Length>0){
                    GameObject target1 = findCanPlay(towerList);
                    if(target1!=null){
                        target1.GetComponent(TowerScript).SetBuddyMode(true);
                    }else{//If it is not within the range that can be helped, the move is performed first, and the move is within the target range
                        GameObject target2 = findNearest(towerList);
                        //Find out after calling the tower to speed up the attack
                        if(target2!=null){
                            //xecute the move function,but don't know how moved it, so just call it by the method before
                            pos=Pathfinding.Pathfinder.GetPath(target2.transform.position,temp.location);
                        }
                    }
                }

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

        //You can move the buddy using rig.MovePosition(newPosition)
        //You can get the current position of the buddy using transform.position, so to move it use rig.MovePosition(transform.position + direction)
        //When you are close enough to a point (e.g (currentTarget - transform.position).magnitude < 0.05f), start moving to the next point in the path.
        //If you have visited every point in the path, the action is complete
        if (!isok && pos != null)
        {
            rig.MovePosition(transform.position + directionVector * Time.fixedDeltaTime);
            if (Vector3.Distance(new Vector2(transform.position.x,transform.position.z), new Vector2(pos[Currentpos].x,pos[Currentpos].z)) < 0.05f)
            {
                Currentpos++;
                if (Currentpos >= pos.Length)
                {
                    isok = true;
                    Currentpos = -1;
                }
                else
                {
                    directionVector = getNewDirectionVector(pos[Currentpos]);
                }
            }
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

    GameObject void findNearest(GameObject[] enemiesList){
        float dis1 = 9999;//Its own attack range, this can be modified, if the enemy is larger than this range then there is no target to attack
        GameObject target;
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
