using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Should be attached to physical objects which can be moved by the player during the battle phase of the game
/// </summary>
public class RuntimeMovable : MonoBehaviour
{
    Vector3 lastPosition;
    Quaternion lastRotation;
    /// <summary>
    /// The minimum distance from the object's previous position which will cause it to register as moved
    /// </summary>
    [SerializeField] float MovementThreshold = 0.05f;
    /// <summary>
    /// The minimum angle from the object's previous position which will cause it to register as moved
    /// </summary>
    [SerializeField] float RotationThreshold = 15;
    /// <summary>
    /// A script which implements IRuntimeMovableBehaviourScript that controls the normal behaviour of the object e.g TowerScript, WallScript
    /// </summary>
    [SerializeField] MonoBehaviour behaviourScript;
    /// <summary>
    /// How long the penalties for moving the object are applied for
    /// </summary>
    [SerializeField] float PenaltyDuration = 5f;
    
    IRuntimeMovableBehaviourScript _behaviourScript = null;
    Pathfinding.PathfindingObstacle obstacle;

    // Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.position;
        lastRotation = transform.rotation;
        if (behaviourScript != null)_behaviourScript = (IRuntimeMovableBehaviourScript)behaviourScript;
        obstacle = GetComponent<Pathfinding.PathfindingObstacle>();
    }

    // Update is called once per frame
    void Update()
    {
        if(((transform.position - lastPosition).magnitude > MovementThreshold || System.Math.Abs(Quaternion.Angle(transform.rotation, lastRotation)) > RotationThreshold) && GameProperties.BattlePhase)
        {
            StartCoroutine(penaliseMovementThenStartAgain());
            lastPosition = transform.position;
            lastRotation = transform.rotation;

        }
        else if (!GameProperties.BattlePhase)
        {
            lastPosition = transform.position;
            lastRotation = transform.rotation;
        }
    }

    IEnumerator penaliseMovementThenStartAgain()
    {
        _behaviourScript?.ApplyMovementPenalty();
        obstacle?.SetSendsNodes(false);
        yield return new WaitForSeconds(PenaltyDuration);
        _behaviourScript?.EndMovementPenalty();
        obstacle?.SetSendsNodes(true);
        yield return null;
    }
}
