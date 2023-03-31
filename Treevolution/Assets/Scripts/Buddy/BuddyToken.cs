using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TOKEN_TYPES { Action, Subject, Connective, Error }
public abstract class BuddyToken
{
    public readonly TOKEN_TYPES tokenType;

    public BuddyToken(TOKEN_TYPES tokenType)
    {
        this.tokenType = tokenType;
    }
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

public class ConnectiveBuddyToken : BuddyToken
{
    public ConnectiveBuddyToken() : base(TOKEN_TYPES.Connective) { }
}
