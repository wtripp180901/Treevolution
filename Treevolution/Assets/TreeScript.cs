using UnityEngine;

public class TreeScript : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private int health = 10000;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<EnemyScript>().DestroyEnemy(false);
            health -= 1;
        }
        if (health == 0)
        {
            //Destroy(gameObject);
            //GameObject.FindWithTag("Logic").GetComponent<GameStateManager>().GameOverScreen(false);
        }
    }

}
