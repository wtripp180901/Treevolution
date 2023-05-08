using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Towers
{
    public class TowerManager : MonoBehaviour
    {
        /// <summary>
        /// List of currently active towers.
        /// </summary>
        private List<GameObject> _activeTowers;
        /// <summary>
        /// Toggle determining whether all towers should display their range visuals or not.
        /// </summary>
        private bool _rangeVisualToggle = true;
        private bool _disabledToggle = true;

        // Start is called before the first frame update
        void Start()
        {
            _activeTowers = new List<GameObject>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Toggles all of the towers' range visuals at once, in accordance with the provided toggle.
        /// </summary>
        /// <param name="toggle">True to display all towers' range visuals and False to hide them.</param>
        public void ToggleAllRangeVisuals(bool showRange)
        {
            _rangeVisualToggle = showRange;
            foreach (GameObject tower in _activeTowers)
            {
                TowerScript towerScript = tower.GetComponent<TowerScript>();
                towerScript.DisplayRange(_rangeVisualToggle);
            }
        }

        /// <summary>
        /// Sets whether to disable all towers from shooting
        /// </summary>
        /// <param name="disableTowers">True to disable towers and False to enable them</param>
        public void DisableAllTowers(bool disableTowers)
        {
            _disabledToggle = disableTowers;
            foreach (GameObject tower in _activeTowers)
            {
                TowerScript towerScript = tower.GetComponent<TowerScript>();
                towerScript.ShootingDisabled(disableTowers);
            }
        }

        /// <summary>
        /// Adds a tower for the TowerManager to manage.
        /// </summary>
        /// <param name="tower"></param>
        public void AddTower(GameObject tower)
        {
            if (!_activeTowers.Contains(tower))
            {
                tower.GetComponent<TowerScript>().DisplayRange(_rangeVisualToggle);
                _activeTowers.Add(tower);
            }
        }

        /// <summary>
        /// Removes a tower for the TowerManager's managed towers.
        /// </summary>
        /// <param name="tower"></param>
        public void RemoveTower(GameObject tower)
        {
            if (_activeTowers.Contains(tower))
            {
                _activeTowers.Remove(tower);
            }
        }
    }
}
