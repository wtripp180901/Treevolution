using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeScript : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private int health = 5;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Destroy(collision.gameObject);
            health -= 1;
        }
        if (health == 0)
        {
            Destroy(gameObject);
            GameObject.FindWithTag("MainCamera").GetComponent<PhaseTransition>().GameOverScreen(false);
        }
    }

}
