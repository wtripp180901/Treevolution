using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScoreUpdatedEventArgs : EventArgs
{
    public readonly int ScoreChangedBy;
    public ScoreUpdatedEventArgs(int ScoreChangedBy)
    {
        this.ScoreChangedBy = ScoreChangedBy;
    }
}

public class GameStateManager : MonoBehaviour
{
    public enum GameStates { PLAYING, PAUSED, ROUND_FINISHED }
    public static event EventHandler<EventArgs> RoundOverEvent;
    public static event EventHandler<EventArgs> TogglePauseEvent;
    public static event EventHandler<EventArgs> ScoreUpdatedEvent;

    private int _Score = 0;
    public int Score { get { return _Score; } }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddScore(int score)
    {
        _Score += score;
        if (ScoreUpdatedEvent != null) ScoreUpdatedEvent.Invoke(this,new ScoreUpdatedEventArgs(score));
    }
}
