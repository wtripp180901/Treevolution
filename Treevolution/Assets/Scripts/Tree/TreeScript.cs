using UnityEngine;


/// <summary>
/// Manages enemy interactions with the tree
/// </summary>
public class TreeScript : MonoBehaviour
{
    // Start is called before the first frame update

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<EnemyScript>().DestroyEnemy(false);
        }
    }

}
