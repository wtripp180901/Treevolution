using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

public class PointerLocationTracker : MonoBehaviour
{
    public GameObject pointer;
    private int mask = 1 << 8;
    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (IMixedRealityInputSource s in CoreServices.InputSystem.DetectedInputSources)
        {
            if (s.SourceType == InputSourceType.Hand)
            {
                foreach (IMixedRealityPointer p in s.Pointers)
                {
                    if (!(p is IMixedRealityNearPointer) && p.Result != null)
                    {
                        //pointer.transform.position = p.Result.Details.Point;
                        Ray ray = new Ray(p.Position, p.Result.Details.Point - p.Position);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
                        {
                            pointer.transform.position = hit.point;
                        }
                    }
                }
            }
        }
    }
}
