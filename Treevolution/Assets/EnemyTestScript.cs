using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTestScript : MonoBehaviour
{
    public float Speed = 0.0001f;
    public Vector3 rotation;
    public Transform[] target;
    private int current;
    public float rotationSpeed = 0.1f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, 0, verticalInput);
        direction.Normalize();
        transform.Translate(direction * Speed * Time.deltaTime, Space.World);
        if (direction != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(-direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(toRotation, transform.rotation, rotationSpeed * Time.deltaTime);
        }
            
        /* if (transform.position != target[current].position)
         {
             Vector3 pos = Vector3.MoveTowards(transform.position, target[current].position, Speed * Time.deltaTime);
             GetComponent<Rigidbody>().MovePosition(pos);
         }
         else
             current = (current + 1) % target.Length;
     */
    }
        

     /*   void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
            Destroy(collision.gameObject);
    }
     */
}