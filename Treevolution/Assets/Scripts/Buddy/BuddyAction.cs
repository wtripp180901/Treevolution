using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BUDDY_ACTION_TYPES { Move, Error, Attack }
public enum BUDDY_SUBJECT_TYPES { PointerLocation, SingleClosestToPointer, GroupCloseToPointer, Unresolved, Error}

public abstract class BuddyAction
{
    public readonly BUDDY_ACTION_TYPES actionType;

    public BuddyAction(BUDDY_ACTION_TYPES type)
    {
        this.actionType = type;
    }
}

public class MoveBuddyAction : BuddyAction
{
    public readonly Vector3 location;
    public MoveBuddyAction(Vector3 location) : base(BUDDY_ACTION_TYPES.Move)
    {
        this.location = location;
    }
}

/*public abstract class TargettedBuddyAction : BuddyAction
{
    public readonly bool MultiTarget;
    public TargettedBuddyAction(BUDDY_ACTION_TYPES type,bool MultiTarget) : base(type) {
        this.MultiTarget = MultiTarget;
    }
}*/

public class TargetedBuddyAction : BuddyAction
{
    public readonly GameObject[] targets;

    public TargetedBuddyAction(BUDDY_ACTION_TYPES actionType,GameObject[] targets) : base(actionType)
    {
        this.targets = targets;
    }

    public TargetedBuddyAction(BUDDY_ACTION_TYPES actionType, GameObject target) : base(actionType)
    {
        this.targets = new GameObject[] { target };
    }
}

/*public class SingleTargetBuddyAction : TargettedBuddyAction
{
    public readonly GameObject target;

    public SingleTargetBuddyAction(BUDDY_ACTION_TYPES actionType, GameObject target) : base(actionType, false)
    {
        this.target = target;
    }
}*/
