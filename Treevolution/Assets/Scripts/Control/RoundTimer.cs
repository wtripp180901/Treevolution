using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class RoundTimer : MonoBehaviour
{

    public bool isPause=false;
    public bool isStop=false;

    UnityEvent _SecondPassedEvent=new UnityEvent();
    UnityEvent _RounOverEvent=new UnityEvent();
    UnityEvent _StopTimer = new UnityEvent();
    public void SecondPassedEvent()
    {
        _SecondPassedEvent?.Invoke();
    }
    public void RounOverEvent()
    {
        _RounOverEvent?.Invoke();
        _RounOverEvent=null;
    }


    void Start()
    {
        _SecondPassedEvent.AddListener(()=>Debug.LogError("Execution per second")) ;
        _RounOverEvent.AddListener(()=>Debug.LogError("60 second execution")) ;
        _StopTimer.AddListener(()=>Debug.LogError("end")) ;
        StartCoroutine(Timer(60));
    }
    public float timer = 0.0f;
    public float _timer = 0.0f;
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            PauseTimeer();
        }

        if (isPause||isStop)
        {
            _timer = 0;
            return;

        }

        _timer += Time.deltaTime;
        timer += Time.deltaTime;


        if (_timer >= 1f)
        {
            _timer = 0;
            SecondPassedEvent();
        }


        if (timer >= 60)
        {
            RounOverEvent();
        }



    }
    IEnumerator Timer(float time)
    {
        while (true&&!isPause)
        {
            yield return new WaitForSeconds(1.0f);

            SecondPassedEvent();
            yield return new WaitForSeconds(time);
            RounOverEvent();

        }
    }
    //ͣStop timer
    public void StopTimeer()
    {
        isStop = true;
        _StopTimer?.Invoke();
    }
    //Pause timer
    public void PauseTimeer()
    {
        isPause=true;
        _StopTimer?.Invoke();
    }

}
