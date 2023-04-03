using UnityEngine;

public abstract class TowerScript : MonoBehaviour
{
    [SerializeField]
    public float rangeRadius;
    [SerializeField]
    public float fireRate;
    [HideInInspector]
    public EnemyManager enemyManager;
    [HideInInspector]
    public GameObject rangeVisual;
    [SerializeField]
    public Material rangeVisualMaterial;
    [HideInInspector]
    private TowerManager towerManager;

    public void Start()
    {
        GameObject logic = GameObject.FindGameObjectWithTag("Logic");
        towerManager = logic.GetComponent<TowerManager>();
        enemyManager = logic.GetComponent<EnemyManager>();
        rangeVisual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rangeVisual.GetComponent<MeshRenderer>().material = rangeVisualMaterial;
        rangeVisual.GetComponent<Collider>();
        GameObject.Destroy(rangeVisual.GetComponent<Collider>());
        DisplayRange(true);
        towerManager.AddTower(this.gameObject);
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



