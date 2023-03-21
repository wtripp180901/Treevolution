using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QRSimulation : MonoBehaviour
{
    PlaneMapper planeMapper;
    // Start is called before the first frame update
    void Start()
    {
        planeMapper = GameObject.FindGameObjectWithTag("Logic").GetComponent<PlaneMapper>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enabled)
        {
            float p = Random.Range(0.0f, 1.0f);
            if (p > 0.99f) {
                //Vector3 jiggle = new Vector3(-90 + Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5));
                //gameObject.transform.rotation = Quaternion.Euler(jiggle.x, jiggle.y, jiggle.z);
                Pose pose = new Pose(gameObject.transform.position, gameObject.transform.rotation);
                planeMapper.CreateNewPlane(gameObject.transform.position, pose);
            }
            
        }
    }
}
