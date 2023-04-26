using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmergencyKeyboard : MonoBehaviour
{
    public TouchScreenKeyboard keyboard;

    // Start is called before the first frame update
    void Start()
    {
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
    }

    // Update is called once per frame
    void Update()
    {
        if(keyboard != null)
        {
            if (keyboard.text.Contains('\n') || keyboard.text.Contains('.'))
            {
                GetComponent<PointerLocationTracker>().StartSampling();
                GetComponent<VoiceCommandReceiver>().ProcessDictation(keyboard.text.Substring(0, keyboard.text.Length - 1));
                keyboard.text = "";
            }
            if (keyboard.active)
            {
                GetComponent<UIController>().ShowDictation(keyboard.text);
            }
            else
            {
                GetComponent<UIController>().ShowDictation("inactive");
                keyboard.active = true;
            }
        }
        else
        {
            GetComponent<UIController>().ShowDictation("keyboard is null");
            keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
        }
    }
}
