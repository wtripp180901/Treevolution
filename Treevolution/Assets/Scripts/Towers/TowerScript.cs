using UnityEngine;
using System.Collections;

namespace Towers
{
    public abstract class TowerScript : MonoBehaviour, IRuntimeMovableBehaviourScript
    {
        public float rangeRadius = 0.2f;
        public float fireRate = 0.5f;
        public int damage = 1;
        public Material rangeVisualMaterial;
        public GameObject disabledVisualObject;
        public Material enabledMaterial;
        public Material disabledMaterial;
        [HideInInspector]
        public EnemyManager enemyManager;
        [HideInInspector]
        public GameObject rangeVisual;

        private TowerManager _towerManager;
        protected bool shootingDisabled = false;

        float baseFireRate;

        public void Start()
        {
            GameObject logic = GameObject.FindGameObjectWithTag("Logic");
            _towerManager = logic.GetComponent<TowerManager>();
            enemyManager = logic.GetComponent<EnemyManager>();
            rangeVisual = GetRangeObject();
            rangeVisual.GetComponent<MeshRenderer>().material = rangeVisualMaterial;
            rangeVisual.GetComponent<Collider>();
            GameObject.Destroy(rangeVisual.GetComponent<Collider>());
            DisplayRange(true);
            _towerManager.AddTower(this.gameObject);
            baseFireRate = fireRate;
        }
        public void Update()
        {
            UpdateTargets();
            if (rangeVisual.activeInHierarchy)
            {
                UpdateRangeVisual();
            }
        }

        public abstract void UpdateRangeVisual();
        public abstract GameObject GetRangeObject();

        public void DisplayRange(bool toggle)
        {
            if (toggle && !shootingDisabled)
            {
                rangeVisual.SetActive(true);
            }
            else if (rangeVisual != null)
            {
                rangeVisual.SetActive(false);
            }
        }
        public abstract void Attack();
        public abstract void UpdateTargets();
        public float DistToTarget(Vector3 target)
        {
            float distance = Vector3.Distance(target, transform.position);
            return distance;
        }
        public float DistToTargetTopDown(Vector3 target)
        {
            Vector2 topDownTarget = new Vector2(target.x, target.z);
            float distance = Vector2.Distance(topDownTarget, new Vector2(transform.position.x, transform.position.z));
            return distance;
        }
        private void OnDestroy()
        {
            _towerManager.RemoveTower(gameObject);
            Destroy(rangeVisual);
        }


        public void ApplyMovementPenalty()
        {
            shootingDisabled = true;
            disabledVisualObject.GetComponent<Renderer>().material = disabledMaterial;
        }

        public void EndMovementPenalty()
        {
            shootingDisabled = false;
            disabledVisualObject.GetComponent<Renderer>().material = enabledMaterial;

        }

        public void ShootingDisabled(bool toggle)
        {
            shootingDisabled = toggle;
        }

        //If buddyMode is true, set "fireRate" to a higher "fireRate". If false, return it to the original fireRate
        public void EnterBuddyMode()
        {
            fireRate = baseFireRate * 0.5f;
            StartCoroutine(EndBuddyMode());
        }

        IEnumerator EndBuddyMode()
        {
            yield return new WaitForSeconds(5f);
            fireRate = baseFireRate;
        }
    }
}
