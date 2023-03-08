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
                        /*Vector3 pos =  p.Result.Details.Point;
                        pos = new Vector3(pos.x, GameProperties.FloorHeight + 0.0005f, pos.z);*/
                        pointer.transform.position = p.Result.Details.Point;
                    }
                }
            }
        }
    }
}
