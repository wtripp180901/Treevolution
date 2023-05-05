using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    private List<GameObject> _activeTowers;
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

    public void ToggleAllRangeVisuals(bool showRange)
    {
        _rangeVisualToggle = showRange;
        foreach (GameObject tower in _activeTowers)
        {
            TowerScript towerScript = tower.GetComponent<TowerScript>();
            towerScript.DisplayRange(_rangeVisualToggle);
        }
    }

    public void DisableAllTowers(bool disableTowers)
    {
        _disabledToggle = disableTowers;
        foreach (GameObject tower in _activeTowers)
        {
            TowerScript towerScript = tower.GetComponent<TowerScript>();
            towerScript.ShootingDisabled(disableTowers);
        }
    }

    public void AddTower(GameObject tower)
    {
        if (!_activeTowers.Contains(tower))
        {
            tower.GetComponent<TowerScript>().DisplayRange(_rangeVisualToggle);
            _activeTowers.Add(tower);
        }
    }
    public void RemoveTower(GameObject tower)
    {
        if (_activeTowers.Contains(tower))
        {
            _activeTowers.Remove(tower);
        }
    }
}
