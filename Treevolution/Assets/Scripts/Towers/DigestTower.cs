using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Towers
{
    public class DigestTower : TowerScript
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
        /// Transform of the currently targetted enemy.
        /// </summary>
        private Transform _targetTransform;
        /// <summary>
        /// Distance from the tower to the currently targetted enemy.
        /// </summary>
        private float _distanceToCurrentTarget;
        /// <summary>
        /// Timer to count down between digest events, ensuring that the tower's damage interval is being met.
        /// </summary>
        private float _digestDelta;
        /// <summary>
        /// Indicates if the tower is currently digesting.
        /// </summary>
        private bool _currentlyDigesting = false;
        /// <summary>
        /// Animator component of the tower, to trigger an animation when the plant catches an enemy.
        /// </summary>
        private Animator _animator;

        /// <summary>
        /// Calls the base class' Start, and then initialises its own components.
        /// </summary>
        public void Start()
        {
            base.Start();
            _distanceToCurrentTarget = float.MaxValue;
            _digestDelta = 0;
            _animator = GetComponent<Animator>();
        }

        public void SetupForTest(float radius)
        {
            rangeRadius = radius;
        }

        public void GetTestData(out bool digesting)
        {
            digesting = _currentlyDigesting;
        }

        /// <summary>
        /// Calls the base class' Start, and then attacks if the damage interval time has passed.
        /// </summary>
        private void Update()
        {
            base.Update();
            if (!shootingDisabled && !_currentlyDigesting && _targetTransform != null)
            {
                _digestDelta = _digestDelta - Time.deltaTime;
                if (_digestDelta <= 0)
                {
                    _currentlyDigesting = true;
                    Attack();
                }
            }
            else if (_currentlyDigesting)
            {
                if (_digestDelta <= 0)
                {
                    _currentlyDigesting = false;
                    _distanceToCurrentTarget = float.MaxValue;
                }
                _digestDelta = _digestDelta - Time.deltaTime;
            }
            else
            {
                _distanceToCurrentTarget = float.MaxValue;
            }
        }



        /// <summary>
        /// Triggers the enemy catch animation.
        /// </summary>
        private void TransitionAnimation()
        {
            _animator.SetTrigger("Trigger");
        }

        /// <summary>
        /// Attacks the currently targetted enemy.
        /// </summary>
        public override void Attack()
        {
            TransitionAnimation();
            _digestDelta = fireRate;
            _targetTransform.gameObject.GetComponent<EnemyScript>().Damage(damage);
            //_targetTransform.position = gameObject.transform.position;
            //_targetTransform.gameObject.GetComponent<EnemyScript>().frozen = true;
        }

        /// <summary>
        /// Updates the currently targetted enemy to the closest one in range.
        /// </summary>
        public override void UpdateTargets()
        {
            GameObject[] enemies = enemyManager.enemies;
            for (int i = 0; i < enemies.Length; i++)
            {
                float topDownDistToEnemy = DistToTargetTopDown(enemies[i].transform.position);
                if (topDownDistToEnemy < _distanceToCurrentTarget && topDownDistToEnemy <= rangeRadius)
                {
                    if (!_targetFlying && enemies[i].GetComponent<EnemyScript>().flying)
                        continue; // Ignore flying enemies if specified
                    if (!_targetGround && !enemies[i].GetComponent<EnemyScript>().flying)
                        continue; // Ignore ground enemies if specified
                    _targetTransform = enemies[i].transform;
                    _distanceToCurrentTarget = topDownDistToEnemy;
                }
            }
        }
        /// <summary>
        /// Updates the position and size of the range visual to match the tower's position and attributes.
        /// </summary>
        public override void UpdateRangeVisual()
        {
            rangeVisual.transform.localScale = new Vector3(rangeRadius * 2, 0.3f, rangeRadius * 2);
            rangeVisual.transform.position = new Vector3(transform.position.x, GameProperties.FloorHeight + 0.3f, transform.position.z);
        }

        /// <summary>
        /// Gets the range object of the current tower.
        /// </summary>
        /// <returns>A cylinder object.</returns>
        public override GameObject GetRangeObject()
        {
            return GameObject.CreatePrimitive(PrimitiveType.Cylinder);

        }
    }
}
