using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerScript : MonoBehaviour
{
    [SerializeField] float turretRange = 13f;
    [SerializeField] float turretRotationSpeed = 5f;

    public Transform targetTransform;
    private Gun currentGun;
    private float fireRate;
    private float fireRateDelta;


    // Start is called before the first frame update
    void Start()
    {
        //targetTransform = FindObjectOfType<PlayerController>().transform;
        currentGun = GetComponentInChildren<Gun>();
        fireRate = currentGun.GetRateOfFire();
    }

    // Update is called once per frame
    void Update()
    {
        if (targetTransform != null)
        {
            Vector3 enemyGroundPosition = new Vector3(targetTransform.position.x, transform.position.y, targetTransform.position.z);

            if (Vector3.Distance(transform.position, enemyGroundPosition) > turretRange)
                return;
            Vector3 enemyDirection = enemyGroundPosition - transform.position;
            float turretRotationStep = turretRotationSpeed * Time.deltaTime;
            Vector3 newLookDirection = Vector3.RotateTowards(transform.forward, enemyDirection, turretRotationStep, 0f);
            transform.rotation = Quaternion.LookRotation(newLookDirection);
            fireRateDelta = fireRateDelta - Time.deltaTime;
            if (fireRateDelta <= 0)
            {
                currentGun.Fire();
                fireRateDelta = fireRate;
            }
        }
        else
        {
            GameObject placeHolderEnemy = GameObject.FindGameObjectWithTag("Enemy");
            if(placeHolderEnemy != null)
            {
                targetTransform = placeHolderEnemy.transform;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, turretRange);
    }
}
