using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BUDDY_ACTION_TYPES { Move, Error, Attack }
public enum BUDDY_SUBJECT_TYPES { PointerLocation, Error}

public class BuddyAction
{
    public readonly BUDDY_ACTION_TYPES actionType;
    public readonly Vector3 location;

    public BuddyAction(BUDDY_ACTION_TYPES type,Vector3 location)
    {
        this.actionType = type;
        this.location = location;
    }
}
