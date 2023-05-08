using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    // Start is called before the first frame update
    void Start()
    {
        _activeTowers = new List<GameObject>();
    }

    /// <summary>
    /// Toggles all of the towers' range visuals at once, in accordance with the provided toggle.
    /// </summary>
    /// <param name="toggle">True to display all towers' range visuals and False to hide them.</param>
    public void ToggleAllRangeVisuals(bool toggle)
    {
        _rangeVisualToggle = toggle;
        foreach (GameObject tower in _activeTowers)
        {
            TowerScript towerScript = tower.GetComponent<TowerScript>();
            towerScript.DisplayRange(_rangeVisualToggle);
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
}
