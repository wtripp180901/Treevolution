using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTargetTower : TowerScript
{
    /// <summary>
    /// Determines if the tower can target flying enemies.
    /// </summary>
    [SerializeField]
    private bool _targetFlying = false;
    /// <summary>
    /// Determines if the tower can target ground enemies.
    /// </summary>
    [SerializeField]
    private bool _targetGround = true;
    /// <summary>
    /// Rotation speed of the tower when targetting enemies.
    /// </summary>
    [SerializeField]
    private float _rotationSpeed = 3f;
    /// <summary>
    /// Distance from the tower to the currently targetted enemy.
    /// </summary>
    private float _distanceToCurrentTarget;
    /// <summary>
    /// Transform of the currently targetted enemy.
    /// </summary>
    private Transform _targetTransform;
    /// <summary>
    /// Attached Gun instance of the current tower.
    /// </summary>
    private Gun _currentGun;
    /// <summary>
    /// Timer to count down between damage events, ensuring that the tower's damage interval is being met.
    /// </summary>
    private float _fireRateDelta;
    /// <summary>
    /// The part(s) of the tower which will rotate towards the current target.
    /// </summary>
    [SerializeField]
    private GameObject rotatingObject;

    /// <summary>
    /// Calls the base class' Start, and then initialises its own components.
    /// </summary>
    public void Start()
    {
        base.Start();
        _currentGun = GetComponent<Gun>();
        _distanceToCurrentTarget = float.MaxValue;
        _fireRateDelta = fireInterval;
    }

    public void SetupForTest(float range,GameObject rotating)
    {
        rangeRadius = range;
        rotatingObject = rotating;
    }

    /// <summary>
    /// Calls the base class' Start, and then rotates towards the current target, and attacks if the fire interval time has passed.
    /// </summary>
    private void Update()
    {
        base.Update();
        if (!shootingDisabled && _targetTransform != null)
        {
            Vector3 enemyGroundPosition = _targetTransform.position;
            enemyGroundPosition.y = transform.position.y;
            _distanceToCurrentTarget = DistToTarget(enemyGroundPosition);
            Vector3 enemyDirection = enemyGroundPosition - transform.position;
            float turretRotationStep = _rotationSpeed * Time.deltaTime;
            Vector3 newLookDirection = Vector3.RotateTowards(rotatingObject.transform.forward, enemyDirection, turretRotationStep, 0f);
            rotatingObject.transform.rotation = Quaternion.LookRotation(newLookDirection);
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
    }

    /// <summary>
    /// Fires projectiles at the current target, and sets the fire interval to start counting down.
    /// </summary>
    public override void Attack()
    {
        _currentGun.Fire(_targetTransform, damage);
        _fireRateDelta = fireInterval;
    }

    /// <summary>
    /// Updates the currently targetted enemy to the closest one in range.
    /// </summary>
    public override void UpdateTargets()
    {
        GameObject[] enemies = enemyManager.enemies;
        for (int i = 0; i < enemies.Length; i++)
        {
            float distToEnemy = DistToTarget(enemies[i].transform.position);
            if (distToEnemy < _distanceToCurrentTarget && distToEnemy <= rangeRadius)
            {
                if (!_targetFlying && enemies[i].GetComponent<EnemyScript>().flying)
                    continue; // Ignore flying enemies if specified
                if (!_targetGround && !enemies[i].GetComponent<EnemyScript>().flying)
                    continue; // Ignore ground enemies if specified
                _targetTransform = enemies[i].transform;
                _distanceToCurrentTarget = distToEnemy;
            }
        }
    }

    /// <summary>
    /// Updates the position and size of the range visual to match the tower's position and attributes.
    /// </summary>
    public override void UpdateRangeVisual()
    {
        rangeVisual.transform.localScale = new Vector3(rangeRadius * 2, rangeRadius * 2, rangeRadius * 2);
        rangeVisual.transform.position = transform.position;
    }

    /// <summary>
    /// Gets the range object of the current tower.
    /// </summary>
    /// <returns>A sphere object.</returns>
    public override GameObject GetRangeObject()
    {
        return GameObject.CreatePrimitive(PrimitiveType.Sphere);

    }
}
