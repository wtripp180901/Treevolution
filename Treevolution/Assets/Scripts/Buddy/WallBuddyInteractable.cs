using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Modified BuddyInteractable to be attached to walls to allow them to adjust their constraints when they are broken
/// </summary>
public class WallBuddyInteractable : BuddyInteractable
{
    private WallScript wallScript;

    private void Start()
    {
        wallScript = GetComponent<WallScript>();
    }
    public override bool SatisfiesConstraints(RESTRICTION_TYPES[] hardConstraints, RESTRICTION_TYPES[] softConstraints)
    {
        if (!wallScript.isDestroyed && !wallScript.isDamaged && Array.Exists(BuddySystemProperties, x => x == RESTRICTION_TYPES.DamagedWall))
        {
            List<RESTRICTION_TYPES> temp = new List<RESTRICTION_TYPES>(BuddySystemProperties);
            temp.Remove(RESTRICTION_TYPES.DamagedWall);
            BuddySystemProperties = temp.ToArray();
        }
        if ((wallScript.isDestroyed || wallScript.isDamaged) && !Array.Exists(BuddySystemProperties, x => x == RESTRICTION_TYPES.DamagedWall))
        {
            List<RESTRICTION_TYPES> temp = new List<RESTRICTION_TYPES>(BuddySystemProperties);
            temp.Add(RESTRICTION_TYPES.DamagedWall);
            BuddySystemProperties = temp.ToArray();
        }
        return base.SatisfiesConstraints(hardConstraints, softConstraints);
    }
    public void SetupForTest(RESTRICTION_TYPES[] properties,WallScript wallScript)
    {
        this.wallScript = wallScript;
        SetupForTest(properties);
    }
}
