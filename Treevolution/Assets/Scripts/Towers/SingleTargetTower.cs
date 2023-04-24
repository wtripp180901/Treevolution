using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTargetTower : TowerScript
{
    [SerializeField]
    private float _rotationSpeed = 3f;
    private float _distanceToCurrentTarget;
    private Transform _targetTransform;
    private Gun _currentGun;
    private float _fireRateDelta;


    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        damage = 2;
        _currentGun = GetComponent<Gun>();
        _distanceToCurrentTarget = float.MaxValue;
        _fireRateDelta = fireRate;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTargets();
        if (_targetTransform != null)
        {
            Vector3 enemyGroundPosition = _targetTransform.position;
            enemyGroundPosition.y = transform.position.y;
            _distanceToCurrentTarget = DistToTarget(enemyGroundPosition);
            Vector3 enemyDirection = enemyGroundPosition - transform.position;
            float turretRotationStep = _rotationSpeed * Time.deltaTime;
            Vector3 newLookDirection = Vector3.RotateTowards(gameObject.transform.forward, enemyDirection, turretRotationStep, 0f);
            gameObject.transform.rotation = Quaternion.LookRotation(newLookDirection);
            _fireRateDelta = _fireRateDelta - Time.deltaTime;
            if (_fireRateDelta <= 0)
            {
                Attack();
            }
        }
        else
        {
            _distanceToCurrentTarget = float.MaxValue;
        }
        if (rangeVisual.activeInHierarchy)
        {
            UpdateRangeVisual();
        }
    }

    public override void Attack()
    {
        _currentGun.Fire(_targetTransform, damage);
        _fireRateDelta = fireRate;
    }

    public override void UpdateTargets()
    {
        GameObject[] enemies = enemyManager.enemies;
        for (int i = 0; i < enemies.Length; i++)
        {
            float distToEnemy = DistToTarget(enemies[i].transform.position);
            if (distToEnemy < _distanceToCurrentTarget && distToEnemy <= rangeRadius)
            {
                _targetTransform = enemies[i].transform;
                _distanceToCurrentTarget = distToEnemy;
            }
        }
    }

    private void UpdateRangeVisual()
    {
        rangeVisual.transform.localScale = new Vector3(rangeRadius * 2, rangeRadius * 2, rangeRadius * 2);
        rangeVisual.transform.position = transform.position;
    }
}
