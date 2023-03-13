using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDirectionIndicator : MonoBehaviour
{
    Renderer myRenderer;
    Renderer[] childRenderers;

    // Start is called before the first frame update
    void Start()
    {
        myRenderer = GetComponent<Renderer>();
        childRenderers = transform.GetComponentsInChildren<Renderer>();
        toggleVisible(false);
    }

    public void IndicateDirection()
    {
        toggleVisible(true);
        StartCoroutine(waitThenDisappear());
    }

    IEnumerator waitThenDisappear()
    {
        yield return new WaitForSeconds(1f);
        toggleVisible(false);
    }

    void toggleVisible(bool visible)
    {
        myRenderer.enabled = visible;
        foreach(Renderer r in childRenderers)
        {
            r.enabled = visible;
        }
    }
}
