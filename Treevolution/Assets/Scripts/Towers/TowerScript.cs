using UnityEngine;
using System.Collections;

namespace Towers
{
    public abstract class TowerScript : MonoBehaviour, IRuntimeMovableBehaviourScript
    {
        /// <summary>
        /// Radius of the range that the tower can target enemies from (either Cylinder or Sphere).
        /// </summary>
        public float rangeRadius = 0.2f;
        /// <summary>
        /// Seconds between when the tower can damage enemies.
        /// </summary>
        public float fireRate = 0.5f;
        /// <summary>
        /// Damage that the tower causes to enemies.
        /// </summary>
        public int damage = 1;
        /// <summary>
        /// Material to assign to the object defining the tower's range.
        /// </summary>
        public Material rangeVisualMaterial;
        /// <summary>
        /// Object to assign a material to when the tower is disabled for being moved during battle.
        /// </summary>
        public GameObject disabledVisualObject;
        /// <summary>
        /// Material to assign to the disabled object when it is not disabled (i.e. not during penalty, usually transparent material)
        /// </summary>
        public Material enabledMaterial;
        /// <summary>
        /// Material to assign to the disabled object when it is disabled (i.e. during penalty, usually a red glow)
        /// </summary>
        public Material disabledMaterial;
        /// <summary>
        /// Currently active EnemyManager instance.
        /// </summary>
        [HideInInspector]
        public EnemyManager enemyManager;
        /// <summary>
        /// Object defining the tower's range.
        /// </summary>
        [HideInInspector]
        public GameObject rangeVisual;

        /// <summary>
        /// Currently active TowerManager instance.
        /// </summary>
        private TowerManager _towerManager;
        /// <summary>
        /// Toggles whether shooting is enabled or not.
        /// </summary>
        protected bool shootingDisabled = false;

        /// <summary>
        /// Stores the original fire interval when the buddy is assisting the current tower.
        /// </summary>
        float baseFireRate;

        /// <summary>
        /// Start initialises the Tower's internal parameters, and registers it with the tower manager.
        /// </summary>
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

        public void SetupForTest(float range)
        {
            rangeRadius = range;
        }

        public void Update()
        {
            UpdateTargets();
            if (rangeVisual.activeInHierarchy)
            {
                UpdateRangeVisual();
            }
        }
        /// <summary>
        /// Abstract method for updating the tower's range visual position and size.
        /// </summary>
        public abstract void UpdateRangeVisual();
        /// <summary>
        /// Abstract method for getting the range visual object for the current tower.
        /// </summary>
        /// <returns>The GameObject to be used as the tower's range visual.</returns>
        public abstract GameObject GetRangeObject();

        /// <summary>
        /// Displays the range visual of the tower in accordance with the provided toggle.
        /// </summary>
        /// <param name="toggle">True to display the range visual, and False to hide it.</param>
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
        /// <summary>
        /// Abstract method to define the tower attacking the current target(s).
        /// </summary>
        public abstract void Attack();
        /// <summary>
        /// Abstract method to update the currently targetted enemy/enemies.
        /// </summary>
        public abstract void UpdateTargets();
        /// <summary>
        /// Calculates the distance from the tower to a target.
        /// </summary>
        /// <param name="target">The target's position</param>
        /// <returns>Distance (in metres) from the tower's centre to the target.</returns>
        public float DistToTarget(Vector3 target)
        {
            float distance = Vector3.Distance(target, transform.position);
            return distance;
        }

        /// <summary>
        /// Calculates the distance from the tower to a target from a 2D, top-down perspective.
        /// </summary>
        /// <param name="target">The target's 3D position.</param>
        /// <returns>The 2D distance from the tower's centre to the target.</returns>
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

        /// <summary>
        /// IRuntimeMovableBehaviourScript method to apply a penalty to the tower if it is moved during battle.
        /// </summary>
        public void ApplyMovementPenalty()
        {
            shootingDisabled = true;
            disabledVisualObject.GetComponent<Renderer>().material = disabledMaterial;
        }

        /// <summary>
        /// IRuntimeMovableBehaviourScript method to stop applying a penalty to the tower after being moved during battle.
        /// </summary>
        public void EndMovementPenalty()
        {
            shootingDisabled = false;
            disabledVisualObject.GetComponent<Renderer>().material = enabledMaterial;
        }

        /// <summary>
        /// Sets whether shooting is disabled
        /// </summary>
        /// <param name="toggle"></param>
        public void ShootingDisabled(bool toggle)
        {
            shootingDisabled = toggle;
        }

        /// <summary>
        /// Boosts the tower's fire rate when the buddy is helping it.
        /// If buddyMode is true, it sets "fireInterval" to a higher "fireInterval". If false, return it to the original fireInterval
        /// </summary>
        public void EnterBuddyMode()
        {
            fireRate = baseFireRate * 0.5f;
            StartCoroutine(EndBuddyMode());
        }

        /// <summary>
        /// Maintains buddy mode for 5 seconds before returning to a standard fire rate.
        /// </summary>
        /// <returns></returns>
        IEnumerator EndBuddyMode()
        {
            yield return new WaitForSeconds(5f);
            fireRate = baseFireRate;
        }
    }
}
