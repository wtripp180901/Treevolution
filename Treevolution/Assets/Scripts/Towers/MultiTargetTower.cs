using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiTargetTower : TowerScript
{
    private List<EnemyScript> _targetEnemies;
    private EnemyScript _currentTarget;
    private float _fireRateDelta;


    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        _targetEnemies = new List<EnemyScript>();
        _fireRateDelta = 0;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTargets();
        _fireRateDelta = _fireRateDelta - Time.deltaTime;
        if (_targetEnemies != null && _targetEnemies.Count > 0 && _fireRateDelta <= 0)
        {
            foreach(EnemyScript enemyScript in _targetEnemies)
            {
                if (enemyScript.health > 0)
                {
                    _currentTarget = enemyScript;
                    Attack();
                }
            }
            _fireRateDelta = fireRate;
        }
        else if (_fireRateDelta <= 0)
        {
             _fireRateDelta = fireRate;
        }
        if (rangeVisual.activeInHierarchy)
        {
            UpdateRangeVisual();
        }
    }

    public override void Attack()
    {
        _currentTarget.Damage(damage);
    }

    public override void UpdateTargets()
    {
        GameObject[] enemies = enemyManager.enemies;
        for (int i = 0; i < enemies.Length; i++)
        {
            float distToEnemy = DistToTarget(enemies[i].transform.position);
            if (distToEnemy <= rangeRadius && !_targetEnemies.Contains(enemies[i].GetComponent<EnemyScript>()))
            {
                _targetEnemies.Add(enemies[i].GetComponent<EnemyScript>());
            }
        }
    }

    private void UpdateRangeVisual()
    {
        rangeVisual.transform.localScale = new Vector3(rangeRadius * 2, rangeRadius * 2, rangeRadius * 2);
        rangeVisual.transform.position = transform.position;
    }
}
