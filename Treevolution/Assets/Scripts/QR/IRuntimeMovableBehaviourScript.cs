using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// QR objects which can be moved by the player during the battle phase should implement this interface
/// </summary>
public interface IRuntimeMovableBehaviourScript
{
    /// <summary>
    /// Should in some way nerf or disable the object until EndMovementPenalty is called
    /// </summary>
    void ApplyMovementPenalty();
    /// <summary>
    /// Reverses the affects of ApplyMovementPenalty
    /// </summary>
    void EndMovementPenalty();
}
