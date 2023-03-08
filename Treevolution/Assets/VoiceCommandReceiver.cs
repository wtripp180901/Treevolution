using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceCommandReceiver : MonoBehaviour
{
    public GameObject pointer;
    public void LightningBolt()
    {
        StartCoroutine(Indicator());
    }
    IEnumerator Indicator()
    {
        Color defaultColour = pointer.GetComponent<Renderer>().material.color;
        pointer.GetComponent<Renderer>().material.color = Color.red;
        yield return new WaitForSeconds(0.3f);
        pointer.GetComponent<Renderer>().material.color = defaultColour;
    }
}
