using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class RoundTimer : MonoBehaviour
{
    public float roundLengthSecs = 60;
    private bool isPaused=false;
    private bool isStopped=false;

    public UnityEvent _SecondPassedEvent=new UnityEvent();
    UnityEvent _RoundOverEvent=new UnityEvent();
    UnityEvent _StopTimer = new UnityEvent();
    UnityEvent _PauseTimer = new UnityEvent();

    public void SecondPassedEvent()
    {
        _SecondPassedEvent?.Invoke();
    }
    public void RoundOverEvent()
    {
        _RoundOverEvent?.Invoke();
        _RoundOverEvent=null;
    }

    void Start()
    {
        //_SecondPassedEvent.AddListener(()=>Debug.Log("Execution per second")) ;
        _RoundOverEvent.AddListener(()=>Debug.Log("round over")) ;
        _StopTimer.AddListener(()=>Debug.Log("end")) ;
        _PauseTimer.AddListener(() => Debug.Log("pause/play"));

        StartCoroutine(Timer(60));
        PauseTimer();
    }

    private float roundTimer = 0.0f;
    private float secondTimer = 0.0f;
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            PauseTimer();
        }

        if (isPaused||isStopped)
        {
            secondTimer = 0;
            return;
        }

        secondTimer += Time.deltaTime;
        roundTimer += Time.deltaTime;


        if (secondTimer >= 1f)
        {
            secondTimer = 0;
            SecondPassedEvent();
        }


        if (roundTimer >= roundLengthSecs)
        {
            RoundOverEvent();
        }



    }
    IEnumerator Timer(float time)
    {
        while (true&&!isPaused)
        {
            yield return new WaitForSeconds(1.0f);

            SecondPassedEvent();
            yield return new WaitForSeconds(time);
            RoundOverEvent();

        }
    }
    //ͣStop timer
    public void StopTimer()
    {
        isStopped = true;
        _StopTimer?.Invoke();
    }
    //Pause timer
    public void PauseTimer()
    {
        isPaused=!isPaused;
        _PauseTimer?.Invoke();
    }

}
