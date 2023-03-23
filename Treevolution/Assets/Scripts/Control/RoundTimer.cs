using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class RoundTimer : MonoBehaviour
{
    public float roundLengthSecs = 60;
    private bool isPaused = false;
    public bool IsPaused { get { return isPaused; } }
    private bool isStopped = false;
    private bool hasStarted = false;


    public UnityEvent _SecondTickEvent = new UnityEvent();
    UnityEvent _RoundOverEvent = new UnityEvent();
    UnityEvent _StopTimer = new UnityEvent();
    UnityEvent _PauseTimer = new UnityEvent();
    UnityEvent _StartTimer = new UnityEvent();


    public void SecondTickEvent()
    {
        _SecondTickEvent?.Invoke();
    }
    public void RoundOverEvent()
    {
        _RoundOverEvent?.Invoke();
    }

    void Start()
    {
        //_SecondTickEvent.AddListener(() => Debug.Log("Tick"));
        //_RoundOverEvent.AddListener(() => Debug.Log(roundLengthSecs.ToString() + "s Passed"));
        _RoundOverEvent.AddListener(() => GetComponent<GameStateManager>().EndBattle());
        _StartTimer.AddListener(() => GetComponent<UIController>().ResetTimer((int)roundLengthSecs));
        _StopTimer.AddListener(() => Debug.Log("Timer Stopped"));
        _PauseTimer.AddListener(() => Debug.Log("Pause/Play"));
        PauseTimer();
    }

    private void OnApplicationPause(bool pause)
    {
        return;
        switch (pause)
        {
            case true:
                if (hasStarted && !isStopped)
                    PauseTimer();
                break;
            case false:
                if (hasStarted && isPaused && !isStopped)
                    PauseTimer();
                break;
        }
    }

    public void StartTimer()
    {
        roundTimer = 0;
        secondTimer = 0;
        isStopped = false;
        isPaused = false;
        hasStarted = true;
        _StartTimer?.Invoke();
    }

    private float roundTimer = 0.0f;
    private float secondTimer = 0.0f;
    void Update()
    {

        /*if (Input.GetMouseButtonDown(0))
        {
            PauseTimer();
        }*/

        if (isPaused || isStopped)
        {
            secondTimer = 0;
            return;
        }

        secondTimer += Time.deltaTime;
        roundTimer += Time.deltaTime;

        if (secondTimer >= 1f)
        {
            secondTimer = 0;
            SecondTickEvent();
        }

        if (roundTimer >= roundLengthSecs)
        {
            RoundOverEvent();
        }

    }

    //Stop timer
    public void StopTimer()
    {
        hasStarted = false;
        isStopped = true;
        isPaused = false;
        _StopTimer?.Invoke();
    }
    //Pause timer
    public void PauseTimer()
    {
        isPaused = !isPaused;
        _PauseTimer?.Invoke();
    }

}
