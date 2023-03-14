using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    public TMP_Text infoText;
    int time = 60;

    private void Start()
    {
        UnityEngine.Animations.LookAtConstraint lookAtConstraint = infoText.GetComponent<UnityEngine.Animations.LookAtConstraint>();
        lookAtConstraint.constraintActive = true;
    }

    public void Win()
    {
        infoText.text = "You win!";
    }

    public void Lose()
    {
        infoText.text = "You lose!";
    }

    public void DecreaseTime()
    {
        infoText.text = "" + time;
        time -= 1;
    }
}
