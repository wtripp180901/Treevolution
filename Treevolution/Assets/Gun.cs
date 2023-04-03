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

    public void Fire(int damage)
    {
        if (gunPoint != null)
        {
            GameObject p = Instantiate(projectile, gunPoint.position, gunPoint.rotation);
            p.GetComponent<BulletScript>().damage = damage;
        }
    }
}
