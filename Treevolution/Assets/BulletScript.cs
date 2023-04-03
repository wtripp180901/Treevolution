using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField]
    private float projectileSpeed = 15f;
    public int damage = 1;

    private void Update()   //you can change this to a virtual function for multiple projectile types
    {
        transform.Translate(new Vector3(0f, 0f, projectileSpeed * Time.deltaTime));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Wall" || collision.gameObject.tag == "Tree") Destroy(gameObject);
    }
}
