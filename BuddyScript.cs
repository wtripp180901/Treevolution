using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;

public class BuddyScript : MonoBehaviour
{
    Queue<BuddyAction> actionQueue = new Queue<BuddyAction>();
    Rigidbody rig; //This is what physics are applied to in order to move the object

    bool isok = false;
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
        if (actionQueue.Count != 0)
        {
            if(!isok)return;
            BuddyAction temp= actionQueue.Dequeue();
            if (temp.actionType == BUDDY_ACTION_TYPES.Move)
            {
                isok = false;
                //getpath
                 pos=Pathfinding.Pathfinder.GetPath(transform.position,temp.location);
                Currentpos = 0;
                //StartCoroutine(Delay(2f, () =>
                //{
                    
                //}));
            }
            
        }
    }

    IEnumerator Delay(float v, Action value)
    {
        yield return new WaitForSeconds(v);
        value?.Invoke();
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
            rig.MovePosition(pos[Currentpos]);
            if (Vector3.Distance(transform.localPosition, pos[Currentpos]) < 0.5f)
            {
                Currentpos++;
                if (Currentpos >= pos.Length)
                {
                    isok = true;
                    Currentpos = -1;
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
}
