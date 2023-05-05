using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


/// <summary>
/// Should be attached to any object which can be described to the buddy. Can attach the properties describing this object to BuddySystemProperties field
/// </summary>
public class BuddyInteractable : MonoBehaviour
{
    /// <summary>
    /// A list of descriptors which the buddy will use to resolve this object
    /// </summary>
    [SerializeField] protected RESTRICTION_TYPES[] BuddySystemProperties;


    /// <summary>
    /// Returns true if BuddySystemProperties meets constraints specified
    /// </summary>
    /// <param name="hardConstraints">Properties which BuddySystemProperties MUST contain ALL of</param>
    /// <param name="softConstraints">Properties which, assuming any hard constraints are satisfied, will return true if any are satisfied</param>
    /// <returns>Returns true if constraints satisfied</returns>
    public virtual bool SatisfiesConstraints(RESTRICTION_TYPES[] hardConstraints, RESTRICTION_TYPES[] softConstraints)
    {
        return PropertiesSatisfyConstraints(hardConstraints, softConstraints);
    }

    /// <summary>
    /// Helper function for SatisfiesConstraints
    /// </summary>
    /// <param name="hardConstraints">Properties which BuddySystemProperties MUST contain ALL of</param>
    /// <param name="softConstraints">Properties which, assuming any hard constraints are satisfied, will return true if any are satisfied</param>
    /// <returns>Returns true if constraints satisfied</returns>
    protected bool PropertiesSatisfyConstraints(RESTRICTION_TYPES[] hardConstraints, RESTRICTION_TYPES[] softConstraints)
    {
        foreach (RESTRICTION_TYPES c in hardConstraints)
        {
            if (!BuddySystemProperties.Contains(c)) return false;
        }
        if (softConstraints.Length == 0) return true;
        return softConstraints.Intersect(BuddySystemProperties).Any();
    }

    public void SetupForTest(RESTRICTION_TYPES[] properties)
    {
        BuddySystemProperties = properties;
    }
}
