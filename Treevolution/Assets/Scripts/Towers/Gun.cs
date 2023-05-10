using UnityEngine;

/// <summary>
/// A point on a tower from which projectiles are spawned
/// </summary>
public class Gun : MonoBehaviour
{
    [SerializeField]
    private GameObject projectile;
    [SerializeField] GameObject[] gunPoints;


    public void Fire(Transform target, int damage)
    {
        if (gunPoints != null && gunPoints.Length > 0)
        {
            foreach (GameObject gunPoint in gunPoints)
            {
                GameObject p = Instantiate(projectile, gunPoint.transform.position, Quaternion.LookRotation(target.position-gunPoint.transform.position));
                p.GetComponent<BulletScript>().damage = damage;
            }
        }
    }

    public void SetupForTest(GameObject projectile,GameObject[] gunPoints)
    {
        this.projectile = projectile;
        this.gunPoints = gunPoints;
    }
}
