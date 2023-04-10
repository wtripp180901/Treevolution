using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BuddyInteractable : MonoBehaviour
{
    [SerializeField] private RESTRICTION_TYPES[] BuddySystemProperties;

    public bool SatisfiesConstraints(RESTRICTION_TYPES[] hardConstraints, RESTRICTION_TYPES[] softConstraints)
    {
        foreach (RESTRICTION_TYPES c in hardConstraints)
        {
            if (!BuddySystemProperties.Contains(c)) return false;
        }
        if (softConstraints.Length == 0) return true;
        return softConstraints.Intersect(BuddySystemProperties).Any();
    }
}
