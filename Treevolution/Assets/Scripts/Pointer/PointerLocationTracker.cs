using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;

public class PointerLocationTracker : MonoBehaviour, IMixedRealityGestureHandler
{
    public GameObject pointer;
    private int mask = 1 << 8;

    [SerializeField] private float _ClosenessThreshold = 0.2f;
    public float ClosenessThreshold { get { return _ClosenessThreshold; } }

    void Start()
    {
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityGestureHandler>(this);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (CoreServices.InputSystem != null)
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

    List<Vector3> locationSamples = new List<Vector3>();
    bool isSampling = false;

    /// <summary>
    /// Clears previously sampled pointer locations and locks access to sampling list in preparation for new sampling session
    /// </summary>
    public void StartSampling()
    {
        if (!isSampling)
        {
            locationSamples.Clear();
            isSampling = true;
        }
    }


    /// <summary>
    /// End sampling session, allowing a new one to safely clear previous samples and begin
    /// </summary>
    public void FinishSampling()
    {
        isSampling = false;
        if (locationSamples.Count == 0) locationSamples.Add(pointer.transform.position);
    }


    /// <summary>
    /// Creates a sample of the pointers location at the time of calling. To be called by the DictationHandler's OnDictationHypothesis event
    /// </summary>
    void SampleLocation()
    {
        locationSamples.Add(pointer.transform.position);
        GameObject.FindWithTag("MoveToMarker").transform.position = pointer.transform.position;
    }

    /// <summary>
    /// Returns the pointer location sampled on the wordIndex'th word during the last buddy command recording session
    /// </summary>
    /// <param name="wordIndex">The index of the word in a space separated array of words in the dictation string that you wish to get the corresponding location of</param>
    /// <returns></returns>
    public Vector3 GetSampleAtWordIndex(int wordIndex)
    {
        int index;
        if (wordIndex < locationSamples.Count) index = wordIndex;
        else index = locationSamples.Count - 1;
        Instantiate(GameObject.FindWithTag("MoveToMarker"),locationSamples[index],Quaternion.identity);
        return locationSamples[index];
    }

    public void OnGestureStarted(InputEventData eventData) { }

    public void OnGestureUpdated(InputEventData eventData) { }

    public MixedRealityInputAction tapAction;
    public void OnGestureCompleted(InputEventData eventData)
    {
        if (eventData.MixedRealityInputAction == tapAction)
        {
            SampleLocation();
        }
    }

    public void OnGestureCanceled(InputEventData eventData) { }
}
