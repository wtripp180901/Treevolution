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
        /// <summary>
        /// Triggered when a change is made to the pathfinding environment
        /// </summary>
        public static UnityEvent RefindPathNeededEvent = new UnityEvent();

        /// <summary>
        /// Triggers the RefindPathNeededEvent
        /// </summary>
        public static void NotifyObstacleChanged()
        {
            RefindPathNeededEvent.Invoke();
        }
    }

}
