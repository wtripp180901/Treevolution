using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] GameObject projectile;
    [SerializeField] float rateOfFire = 0.5f;
    [SerializeField] Transform gunPoint;
    // Start is called before the first frame update
    void Start()
    {
        if (gunPoint == null)
            gunPoint = GetComponentInChildren<GunPoint>().transform;
    }

    public float GetRateOfFire()
    {
        return rateOfFire;
    }

    public void Fire()
    {
        Instantiate(projectile, gunPoint.position, gunPoint.rotation);
    }
}
