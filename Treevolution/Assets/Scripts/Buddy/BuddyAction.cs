using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BuddyActions
{

    /// <summary>
    /// A concrete action type to be specified to a BuddyAction
    /// </summary>
    public enum BUDDY_ACTION_TYPES { Move, Error, Attack, Repair, Defend, Buff }

    /// <summary>
    /// The subject the buddy should resolve
    /// </summary>
    public enum BUDDY_SUBJECT_TYPES { PointerLocation, SingleClosestToPointer, GroupCloseToPointer, Unresolved, Error }

    /// <summary>
    /// Base class from which concrete actions are derived
    /// </summary>
    public abstract class BuddyAction
    {
        public readonly BUDDY_ACTION_TYPES actionType;

        public BuddyAction(BUDDY_ACTION_TYPES type)
        {
            this.actionType = type;
        }
    }


    /// <summary>
    /// An action instructing the buddy to move to a Vector3 location
    /// </summary>
    public class MoveBuddyAction : BuddyAction
    {
        public readonly Vector3 location;
        public MoveBuddyAction(Vector3 location) : base(BUDDY_ACTION_TYPES.Move)
        {
            this.location = location;
        }
    }

    /// <summary>
    /// An action which will continue being performed until the user gives new instructions
    /// </summary>
    public class OngoingBuddyAction : BuddyAction
    {
        public OngoingBuddyAction(BUDDY_ACTION_TYPES actionType) : base(actionType)
        {
        }
    }

    /*public abstract class TargettedBuddyAction : BuddyAction
    {
        public readonly bool MultiTarget;
        public TargettedBuddyAction(BUDDY_ACTION_TYPES type,bool MultiTarget) : base(type) {
            this.MultiTarget = MultiTarget;
        }
    }*/

    /// <summary>
    /// An action type which targets a GameObject or GameObjects
    /// </summary>
    public class TargetedBuddyAction : BuddyAction
    {
        public readonly GameObject[] targets;

        public TargetedBuddyAction(BUDDY_ACTION_TYPES actionType, GameObject[] targets) : base(actionType)
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

}
