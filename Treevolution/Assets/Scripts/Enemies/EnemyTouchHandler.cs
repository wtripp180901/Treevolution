using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

public class EnemyTouchHandler : MonoBehaviour, IMixedRealityTouchHandler
{
    private EnemyScript enemyScript;

    public void OnTouchCompleted(HandTrackingInputEventData eventData)
    {

    }

    public void OnTouchStarted(HandTrackingInputEventData eventData)
    {
        Handedness hand = eventData.Handedness;
        float ifc = HandPoseUtils.IndexFingerCurl(hand);
        if ((ifc > 0.7f) || Application.platform == RuntimePlatform.WindowsEditor)
            enemyScript.Damage(5);
        /*if (!GameObject.FindGameObjectWithTag("Logic").GetComponent<RoundTimer>().IsPaused)
        {
            GameObject[] receivers = GameObject.FindGameObjectsWithTag("TargetReceiver");
            for (int i = 0; i < receivers.Length; i++)
            {
                receivers[i].GetComponent<TowerScript>().SetTarget(transform);
            }

            GameObject logic = GameObject.FindGameObjectWithTag("Logic");
            logic.GetComponent<EnemyManager>().targetNewEnemy(gameObject);
        }*/
    }

    public void OnTouchUpdated(HandTrackingInputEventData eventData)
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        enemyScript = GetComponent<EnemyScript>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
