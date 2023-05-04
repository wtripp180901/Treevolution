using System.Collections;
using UnityEngine;

/// <summary>
/// Controls behaviour of spawn arrows which indicate enemy positions
/// </summary>
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

    /// <summary>
    /// Causes the arrow to appear briefly
    /// </summary>
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


    /// <summary>
    /// Sets visibility of GameObject and its children
    /// </summary>
    /// <param name="visible">True if visible</param>
    void toggleVisible(bool visible)
    {
        myRenderer.enabled = visible;
        foreach (Renderer r in childRenderers)
        {
            r.enabled = visible;
        }
    }
}
