using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    public TMP_Text text;

    int time = 60;

    public void Win()
    {
        text.text = "You win!";
    }

    public void Lose()
    {
        text.text = "You lose!";
    }

    public void DecreaseTime()
    {
        text.text = "" + time;
        time -= 1;
    }
}
