using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WallBuddyInteractable : BuddyInteractable
{
    private WallScript wallScript;

    private void Start()
    {
        wallScript = GetComponent<WallScript>();
    }
    public override bool SatisfiesConstraints(RESTRICTION_TYPES[] hardConstraints, RESTRICTION_TYPES[] softConstraints)
    {
        if (!wallScript.isDestroyed && !wallScript.isDamaged) return false;
        return base.SatisfiesConstraints(hardConstraints, softConstraints);
    }
    public void SetupForTest(RESTRICTION_TYPES[] properties,WallScript wallScript)
    {
        this.wallScript = wallScript;
        SetupForTest(properties);
    }
}
