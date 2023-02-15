using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseTransition : MonoBehaviour
{
    public GameObject tree;
    public void GoToGamePhase()
    {
        GetComponent<RealWorldPropertyMapper>().MapProperties();
        GetComponent<EnemyManager>().StartSpawning();
        Vector3 vertical = GameProperties.TopLeftCorner - GameProperties.BottomLeftCorner;
        Vector3 horizontal = GameProperties.TopRightCorner - GameProperties.TopLeftCorner;
        Vector3 treeLocation = GameProperties.BottomRightCorner + (0.5f * vertical) - (0.1f * horizontal) + new Vector3(0,tree.GetComponent<Collider>().transform.localScale.y/2,0);
        Instantiate(tree, treeLocation, Quaternion.identity);
        GetComponent<QRDetection>().StopQR();
        GetComponent<RoundTimer>().PauseTimer();
        Destroy(GameObject.FindWithTag("NextRoundButton"));
    }

    public void GameOverScreen(bool win)
    {
        GetComponent<RoundTimer>().StopTimer();
        GetComponent<EnemyManager>().StopSpawning();
        GameObject[] enemyList = GameObject.FindGameObjectsWithTag("Enemy");
        for(int i = 0;i < enemyList.Length; i++)
        {
            Destroy(enemyList[i]);
        }
        if (win)
        {
            GetComponent<UIController>().Win();
        }else
        {
            GetComponent<UIController>().Lose();
        }
    }
}
