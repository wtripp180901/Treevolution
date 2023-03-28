using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;

public class EnemyTouchHandler : MonoBehaviour, IMixedRealityTouchHandler
{
    public void OnTouchCompleted(HandTrackingInputEventData eventData)
    {

    }

    public void OnTouchStarted(HandTrackingInputEventData eventData)
    {
        if (!GameObject.FindGameObjectWithTag("Logic").GetComponent<RoundTimer>().IsPaused)
        {
            GameObject[] receivers = GameObject.FindGameObjectsWithTag("TargetReceiver");
            for (int i = 0; i < receivers.Length; i++)
            {
                receivers[i].GetComponent<TowerScript>().SetTarget(transform);
            }

            GameObject logic = GameObject.FindGameObjectWithTag("Logic");
            logic.GetComponent<EnemyManager>().targetNewEnemy(gameObject);
        }
    }

    public void OnTouchUpdated(HandTrackingInputEventData eventData)
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
