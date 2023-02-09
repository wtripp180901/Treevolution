using UnityEngine;

public class Shot : MonoBehaviour
{
    public GameObject hitPrefab;
    public GameObject muzzlePrefab;
    public float speed;

    Rigidbody rb;
    Vector3 velocity;

    void Awake()
    {
        TryGetComponent(out rb);
    }

    void Start()
    {
        var muzzleEffect = Instantiate(muzzlePrefab, transform.position, transform.rotation);
        Destroy(muzzleEffect, 5f);
        velocity = transform.forward * speed;
    }

    void FixedUpdate()
    {
        var displacement = velocity * Time.deltaTime;
        rb.MovePosition(rb.position + displacement);
    }

    void OnCollisionEnter(Collision other)
    {
        var hitEffect = Instantiate(hitPrefab, other.GetContact(0).point, Quaternion.identity);
        Destroy(hitEffect, 5f);
        Destroy(gameObject);
    }
}