using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Pathfinding {

    /// <summary>
    /// Responsible for notifying all pathfinding agents when a change in the pathfinding environment has occured
    /// </summary>
    public static class PathfindingUpdatePublisher
    {
        public static UnityEvent RefindPathNeededEvent = new UnityEvent();

        public static void NotifyObstacleChanged()
        {
            RefindPathNeededEvent.Invoke();
        }
    }

}
