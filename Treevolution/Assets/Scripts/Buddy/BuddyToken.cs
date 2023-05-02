using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TOKEN_TYPES { Action, Subject, Connective, Restriction, Error }
public enum RESTRICTION_TYPES { Ant, ArmouredBug, Hornet, Dragonfly, Cockroach, Stagbeetle, Flying, Armoured,Unarmoured,Large,Small,Grounded,Wall,
    DamagedWall,Tree,Plant,Flower,Mushroom,Cactus,Poisonous,Black, Blue, Silver, Brown, Yellow, Green, Trap, Red,Tall}
public abstract class BuddyToken
{
    public readonly TOKEN_TYPES tokenType;

    public BuddyToken(TOKEN_TYPES tokenType)
    {
        this.tokenType = tokenType;
    }
}

public class DummyToken : BuddyToken
{
    public DummyToken(): base(TOKEN_TYPES.Error) { }
}

public class ActionBuddyToken : BuddyToken
{
    public readonly BUDDY_ACTION_TYPES actionType;
    public ActionBuddyToken(BUDDY_ACTION_TYPES actionType) : base(TOKEN_TYPES.Action)
    {
        this.actionType = actionType;
    }
}
public class SubjectBuddyToken : BuddyToken
{

    public readonly BUDDY_SUBJECT_TYPES subjectType;
    public SubjectBuddyToken(BUDDY_SUBJECT_TYPES subjectType) : base(TOKEN_TYPES.Subject)
    {
        this.subjectType = subjectType;
    }
}

public class RestrictionBuddyToken : BuddyToken
{
    public readonly RESTRICTION_TYPES restrictionType;
    public RestrictionBuddyToken(RESTRICTION_TYPES restrictionType) : base(TOKEN_TYPES.Restriction)
    {
        this.restrictionType = restrictionType;
    }
}

public class ConnectiveBuddyToken : BuddyToken
{
    public ConnectiveBuddyToken() : base(TOKEN_TYPES.Connective) { }
}
