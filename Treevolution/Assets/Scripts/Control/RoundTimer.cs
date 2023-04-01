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
    private bool running = false;
    public bool isRunning { get { return running; } }


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
        _RoundOverEvent.AddListener(() => StartCoroutine(GetComponent<GameStateManager>().EndBattle()));
        _StartTimer.AddListener(() => GetComponent<UIController>().ResetTimer((int)roundLengthSecs));
        _StopTimer.AddListener(() => Debug.Log("Timer Stopped"));
        _PauseTimer.AddListener(() => Debug.Log("Pause/Play"));
        PauseTimer();
    }

    public void StartTimer()
    {
        roundTimer = 0;
        secondTimer = 0;
        isStopped = false;
        isPaused = false;
        running = true;
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
            StopTimer();
            RoundOverEvent();
        }

    }

    //Stop timer
    public void StopTimer()
    {
        running = false;
        isStopped = true;
        isPaused = false;
        _StopTimer?.Invoke();
    }
    //Pause timer
    public void PauseTimer()
    {
        isPaused = !isPaused;
        running = !running;
        _PauseTimer?.Invoke();
    }

    public void SetRoundLength(int secs)
    {
        roundLengthSecs = secs;
    }

}
