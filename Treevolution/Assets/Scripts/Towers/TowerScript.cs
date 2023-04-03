using UnityEngine;

public abstract class TowerScript : MonoBehaviour
{
    public float rangeRadius = 0.2f;
    public float fireRate = 0.5f;
    public int damage = 1;
    public Material rangeVisualMaterial;

    [HideInInspector]
    public EnemyManager enemyManager;
    [HideInInspector]
    public GameObject rangeVisual;

    private TowerManager _towerManager;

    public void Start()
    {
        GameObject logic = GameObject.FindGameObjectWithTag("Logic");
        _towerManager = logic.GetComponent<TowerManager>();
        enemyManager = logic.GetComponent<EnemyManager>();
        rangeVisual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rangeVisual.GetComponent<MeshRenderer>().material = rangeVisualMaterial;
        rangeVisual.GetComponent<Collider>();
        GameObject.Destroy(rangeVisual.GetComponent<Collider>());
        DisplayRange(true);
        _towerManager.AddTower(this.gameObject);
    }

    public void DisplayRange(bool toggle)
    {
        if (toggle)
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
    private void OnDestroy()
    {
        Destroy(rangeVisual);
    }

}



