using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigestTower : TowerScript
{
    [SerializeField]
    private bool _targetFlying = false;
    [SerializeField]
    private bool _targetGround = true;
    private Transform _targetTransform;
    private float _distanceToCurrentTarget;
    private float _digestDelta;
    private bool _currentlyDigesting = false;
    private Animator _animator;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        _distanceToCurrentTarget = float.MaxValue;
        _digestDelta = 0;
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTargets();
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
        if (rangeVisual.activeInHierarchy)
        {
            UpdateRangeVisual();
        }
    }



    public void TransitionAnimation()
    {
        _animator.SetTrigger("Trigger");
    }


    public override void Attack()
    {
        TransitionAnimation();
        _digestDelta = fireRate;
        _targetTransform.gameObject.GetComponent<EnemyScript>().Damage(damage);
        //_targetTransform.position = gameObject.transform.position;
        //_targetTransform.gameObject.GetComponent<EnemyScript>().frozen = true;
    }

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

    private void UpdateRangeVisual()
    {
        rangeVisual.transform.localScale = new Vector3(rangeRadius * 2, 0.3f, rangeRadius * 2);
        rangeVisual.transform.position = new Vector3(transform.position.x, GameProperties.FloorHeight + 0.3f, transform.position.z);
    }

    public override GameObject GetRangeObject()
    {
        return GameObject.CreatePrimitive(PrimitiveType.Cylinder);

    }
}
