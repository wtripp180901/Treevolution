using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseTransition : MonoBehaviour
{
    public GameObject tree;
    public GameObject startButton;
    public bool devMode = false;

    private void Start()
    {
        if (devMode)
        {
            startButton.SetActive(true);
        }
    }

    public void GoToGamePhase()
    {
        if (devMode)
        {
            PlaneMapper planeMapper = gameObject.GetComponent<PlaneMapper>();
            planeMapper.CreateNewPlane(new Vector3(0.842f, -0.392f, 1.203f), new Vector3(-1.063f, -0.392f, 2.13f));
            GameProperties.FloorHeight = -0.392f;
            GameObject.FindWithTag("Buddy").GetComponent<BuddyScript>().GiveInstructions(new List<BuddyAction>() { new BuddyAction(BUDDY_ACTION_TYPES.Move, new Vector3(-0.52f, -0.295f, 1.336f)), new BuddyAction(BUDDY_ACTION_TYPES.Move, new Vector3(-0.144f, -0.295f, 1.336f)) });
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
