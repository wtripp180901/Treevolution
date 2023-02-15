using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField] float projectileSpeed = 15f;


    private void Update()   //you can change this to a virtual function for multiple projectile types
    {
        transform.Translate(new Vector3(0f, 0f, projectileSpeed * Time.deltaTime));
    }
    void OnCollisionEnter(Collision collision)
    {

        //  Destroy(collision.gameObject);
        if (collision.gameObject.tag == "Enemy") Destroy(gameObject);
    }
}
