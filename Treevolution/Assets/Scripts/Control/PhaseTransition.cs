using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseTransition : MonoBehaviour
{
    public GameObject tree;
    public bool devMode = false;

    public void GoToGamePhase()
    {
        if (devMode)
        {
            PlaneMapper planeMapper = gameObject.GetComponent<PlaneMapper>();
            Pose newPose = Pose.identity;
            newPose.rotation = Quaternion.LookRotation(newPose.up, newPose.forward);
            planeMapper.CreateNewPlane(new Vector3(0.842f, -0.392f, 1.203f), newPose);//, new Vector3(-1.063f, -0.392f, 2.13f));
        }
        GetComponent<RealWorldPropertyMapper>().MapProperties();
        // Game Start Animation?
        GetComponent<EnemyManager>().StartSpawning();
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
