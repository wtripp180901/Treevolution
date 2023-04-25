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
        if (!_currentlyDigesting && _targetTransform != null)
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
                TransitionAnimation();
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
        if (gameObject.name.Contains("Raff"))
            _animator.SetTrigger("RaffTrigger");
        if (gameObject.name.Contains("Venus"))
            _animator.SetTrigger("VenusTrigger");
    }


    public override void Attack()
    {
        TransitionAnimation();
        _digestDelta = fireRate;
        _targetTransform.gameObject.GetComponent<EnemyScript>().Damage(damage);
        _targetTransform.gameObject.GetComponent<EnemyScript>().frozen = true;
    }

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

    private void UpdateRangeVisual()
    {
        rangeVisual.transform.localScale = new Vector3(rangeRadius * 2, rangeRadius * 2, rangeRadius * 2);
        rangeVisual.transform.position = transform.position;
    }
}
