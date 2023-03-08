using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

public class PointerLocationTracker : MonoBehaviour
{
    public GameObject pointer;
    // Update is called once per frame
    void Update()
    {
        foreach(IMixedRealityInputSource s in CoreServices.InputSystem.DetectedInputSources)
        {
            if(s.SourceType == InputSourceType.Hand)
            {
                foreach(IMixedRealityPointer p in s.Pointers)
                {
                    if(!(p is IMixedRealityNearPointer) && p.Result != null)
                    {
                        pointer.transform.position =  p.Result.Details.Point;
                    }
                }
            }
        }
    }
}
