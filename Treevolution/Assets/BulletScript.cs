using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField]
    private bool splashDamage = false;
    [SerializeField]
    private float projectileSpeed = 15f;
    public int damage = 1;
    private EnemyManager _enemyManager;

    private void Start()
    {
        _enemyManager = GameObject.FindGameObjectWithTag("Logic").GetComponent<EnemyManager>();
    }

    private void Update()   //you can change this to a virtual function for multiple projectile types
    {
        transform.Translate(new Vector3(0f, 0f, projectileSpeed * Time.deltaTime));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Tree") 
        {
            if (splashDamage)
            {
                gameObject.GetComponent<ParticleSystem>().Emit(10);
                gameObject.GetComponent<ParticleSystem>().Play();

                foreach (GameObject e in _enemyManager.enemies)
                {
                    if (Vector3.Distance(e.transform.position, gameObject.transform.position) <= 0.1)
                    {
                        e.GetComponent<EnemyScript>().Damage(damage);
                    }
                }
            }
            Destroy(gameObject); 
        }

    }
}
