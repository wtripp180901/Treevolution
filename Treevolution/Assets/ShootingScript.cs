
using UnityEngine;

public class ShootingScript : MonoBehaviour
{
    // Start is called before the first frame update
    public float damage = 10f;
    public float shootingRate = 3;
    public float timer = 0;


    // Update is called once per frame
    void Update()
    {
        if (timer < shootingRate)
            timer = timer + Time.deltaTime;
        else {
            enemyShoot();
            timer = 0;
        }
    }

    void enemyShoot() {

    }
}
