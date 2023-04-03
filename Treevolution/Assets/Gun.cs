using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField]
    private GameObject projectile;
    [SerializeField] Transform gunPoint;
    // Start is called before the first frame update
    void Start()
    {
        gunPoint = GetComponentInChildren<GunPoint>().transform;
    }

    public void Fire()
    {
        if(gunPoint != null)
            Instantiate(projectile, gunPoint.position, gunPoint.rotation);
    }
}
