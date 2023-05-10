using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Towers
{
    /// <summary>
    /// Controls towers which damage enemies within an area of effect
    /// </summary>
    public class MultiTargetTower : TowerScript
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
        /// List of enemy EnemyScripts of the currently targetted enemies.
        /// </summary>
        private List<EnemyScript> _targetEnemies;
        /// <summary>
        /// Current EnemyScript of the target being damaged.
        /// </summary>
        private EnemyScript _currentTarget;
        /// <summary>
        /// Timer to count down between damage events, ensuring that the tower's damage interval is being met.
        /// </summary>
        private float _fireRateDelta;


        /// <summary>
        /// Calls the base class' Start, and then initialises its own components.
        /// </summary>
        public void Start()
        {
            base.Start();
            _targetEnemies = new List<EnemyScript>();
            _fireRateDelta = 0;
        }

        /// <summary>
        /// Calls the base class' Update, and then attacks current targets if the fire interval time has passed.
        /// </summary>
        private void Update()
        {
            base.Update();
            _fireRateDelta = _fireRateDelta - Time.deltaTime;
            if (!shootingDisabled && _targetEnemies != null && _targetEnemies.Count > 0 && _fireRateDelta <= 0)
            {
                foreach (EnemyScript enemyScript in _targetEnemies)
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
        }

        /// <summary>
        /// Damages the current target.
        /// </summary>
        public override void Attack()
        {
            _currentTarget.Damage(damage);
        }

        /// <summary>
        /// Updates the currently targetted enemies to any enemies within range.
        /// </summary>
        public override void UpdateTargets()
        {
            GameObject[] enemies = enemyManager.enemies;
            for (int i = 0; i < enemies.Length; i++)
            {
                float distToEnemy = DistToTarget(enemies[i].transform.position);
                if (distToEnemy <= rangeRadius && !_targetEnemies.Contains(enemies[i].GetComponent<EnemyScript>()))
                {
                    if (!_targetFlying && enemies[i].GetComponent<EnemyScript>().flying)
                        continue; // Ignore flying enemies if specified
                    if (!_targetGround && !enemies[i].GetComponent<EnemyScript>().flying)
                        continue; // Ignore ground enemies if specified
                    _targetEnemies.Add(enemies[i].GetComponent<EnemyScript>());
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
}
