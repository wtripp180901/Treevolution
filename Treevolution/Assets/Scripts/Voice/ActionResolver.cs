using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace LanguageParsing
{
    /// <summary>
    /// Maps data provided by LanguageParser to concrete objects in the scene and returns them as BuddyActions
    /// </summary>
    class ActionResolver
    {
        int moveActionIndex = 0;
        
        /// <summary>
        /// Maps data provided by LanguageParser to concrete objects in the scene and returns them as BuddyActions
        /// </summary>
        /// <param name="action"></param>
        /// <param name="subject"></param>
        /// <param name="restrictions"></param>
        /// <returns></returns>
        public BuddyAction ResolveAction(BUDDY_ACTION_TYPES action,BUDDY_SUBJECT_TYPES subject,RESTRICTION_TYPES[] restrictions)
        {
            if(action == BUDDY_ACTION_TYPES.Move)
            {
                return new MoveBuddyAction(getMoveSubject(subject));
            }
            else
            {
                Vector3 buddyPos = GameObject.FindWithTag("Buddy").transform.position;
                GameObject[] subjects = getSubject(subject, action,restrictions);
                Array.Sort(subjects, new ClosestToBuddyComparer(buddyPos));
                return new TargetedBuddyAction(action, subjects);
            }
        }

        private class ClosestToBuddyComparer : IComparer<GameObject>
        {
            Vector3 buddyPos;
            public ClosestToBuddyComparer(Vector3 buddyPos)
            {
                this.buddyPos = buddyPos;
            }
            public int Compare(GameObject x, GameObject y)
            {
                float distX = (x.transform.position - buddyPos).magnitude;
                float distY = (y.transform.position - buddyPos).magnitude;
                if (distX > distY) return 1;
                else if (distY > distX) return -1;
                else return 0;
            }
        }

        Vector3 getMoveSubject(BUDDY_SUBJECT_TYPES subjectType)
        {
            switch (subjectType)
            {
                case BUDDY_SUBJECT_TYPES.Unresolved:
                case BUDDY_SUBJECT_TYPES.PointerLocation:
                    return getNextPointerSample();
            }
            Debug.Log("Not implemented in getMoveSubject: " + subjectType.ToString());
            return new Vector3(0, 0, 0);
        }

        GameObject[] getSubject(BUDDY_SUBJECT_TYPES subject,BUDDY_ACTION_TYPES action,RESTRICTION_TYPES[] restrictions)
        {
            GameObject[] possibleSubjects = filterByRestrictions(defaultTargetsOfActionType(action), restrictions);
            switch (subject)
            {
                case BUDDY_SUBJECT_TYPES.Unresolved:
                    return possibleSubjects;
                case BUDDY_SUBJECT_TYPES.SingleClosestToPointer:
                    GameObject closest = closestGameObjectToPointer(action,possibleSubjects);
                    if (closest != null) return new GameObject[] { closest };
                    else return new GameObject[] { };
                case BUDDY_SUBJECT_TYPES.GroupCloseToPointer:
                    float closenessThreshold = GameObject.FindWithTag("Logic").GetComponent<PointerLocationTracker>().ClosenessThreshold;
                    Vector3 pointer = getNextPointerSample();
                    return possibleSubjects.Where(x => (x.transform.position - pointer).magnitude < closenessThreshold).ToArray();

            }
            Debug.Log("Not implemented in getSubject: " + subject.ToString());
            return null;
        }

        GameObject[] defaultTargetsOfActionType(BUDDY_ACTION_TYPES action)
        {
            switch (action)
            {
                case BUDDY_ACTION_TYPES.Attack:
                    return GameObject.FindWithTag("Logic").GetComponent<EnemyManager>().enemies;
            }
            Debug.Log("Not implemented in defaultTargetsOfAction: " + action.ToString());
            return null;
        }

        GameObject[] filterByRestrictions(GameObject[] baseList, RESTRICTION_TYPES[] restrictions)
        {
            return baseList.Where(x => x.GetComponent<BuddyInteractable>().SatisfiesConstraints(restrictions,new RESTRICTION_TYPES[] { })).ToArray();
        }

        Vector3 getNextPointerSample()
        {
            Vector3 pointer = GameObject.FindWithTag("Logic").GetComponent<PointerLocationTracker>().GetSampleAtWordIndex(moveActionIndex);
            moveActionIndex++;
            return pointer;
        }

        GameObject closestGameObjectToPointer(BUDDY_ACTION_TYPES action,GameObject[] possibleTargets)
        {
            if (possibleTargets.Length > 0)
            {
                Vector3 pointerPos = getNextPointerSample();
                GameObject closest = possibleTargets[0];
                float closestDist = (closest.transform.position - pointerPos).magnitude;
                for (int i = 0; i < possibleTargets.Length; i++)
                {
                    float currentDist = (possibleTargets[i].transform.position - pointerPos).magnitude;
                    if (currentDist < closestDist)
                    {
                        closest = possibleTargets[i];
                        closestDist = currentDist;
                    }
                }
                return closest;
            }
            else return null;
        }
    }
}
