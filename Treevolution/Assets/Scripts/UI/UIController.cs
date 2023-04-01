using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    public TMP_Text infoText;
    public TMP_Text dictationText;
    int time = 60;
    private EnemyManager enemyManager;

    private void Start()
    {
        UnityEngine.Animations.LookAtConstraint lookAtConstraint = infoText.GetComponent<UnityEngine.Animations.LookAtConstraint>();
        lookAtConstraint.constraintActive = true;
        enemyManager = GetComponent<EnemyManager>();
    }

    public void Win()
    {
        string enemiesKilled = enemyManager.getEnemiesKilled().ToString() ;
        infoText.text = "You win!\nEnemies Killed: " + enemiesKilled;
    }

    public void Lose()
    {
        string enemiesKilled = enemyManager.getEnemiesKilled().ToString();
        infoText.text = "You lose!\nEnemies Killed: " + enemiesKilled;
    }

    public void DecreaseTime()
    {
        infoText.text = "" + time;
        time -= 1;
    }

    public void ShowDictation(string dictation)
    {
        dictationText.text = dictation;
        StartCoroutine(clearTextAfterDelay(3, dictationText));
    }

    IEnumerator clearTextAfterDelay(int delay,TMP_Text text)
    {
        yield return new WaitForSeconds(delay);
        text.text = "";
    }
}
