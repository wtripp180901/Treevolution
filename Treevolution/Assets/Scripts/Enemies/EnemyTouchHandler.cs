using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

/// <summary>
/// Handles user touching the GameObject this script is attached to (via NearInteractionTouchable).
/// Implements IMixedRealityTouchHandler interface.
/// </summary>
public class EnemyTouchHandler : MonoBehaviour, IMixedRealityTouchHandler
{
    /// <summary>
    /// Running EnemyManager instance.
    /// </summary>
    private EnemyScript _enemyScript;

    /// <summary>
    /// Interface method called when touch event is completed (not used).
    /// </summary>
    /// <param name="eventData">Touch event data.</param>
    public void OnTouchCompleted(HandTrackingInputEventData eventData)
    {

    }

    /// <summary>
    /// Interface method called when touch event is started. Used to check if the user's hand is in a fist and if so damage the enemy this script is attached to.
    /// </summary>
    /// <param name="eventData">Touch event data.</param>
    public void OnTouchStarted(HandTrackingInputEventData eventData)
    {
        Handedness hand = eventData.Handedness;
        float ifc = HandPoseUtils.IndexFingerCurl(hand);
        if ((ifc > 0.7f) || Application.platform == RuntimePlatform.WindowsEditor)
            _enemyScript.Damage(5);
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

    /// <summary>
    /// Interface method called when a touch event is updated (not used).
    /// </summary>
    /// <param name="eventData"></param>
    public void OnTouchUpdated(HandTrackingInputEventData eventData)
    {

    }

    /// <summary>
    /// Start runs when loading the GameObject that this script is attached to.
    /// </summary>
    void Start()
    {
        _enemyScript = GetComponent<EnemyScript>();
    }
}
