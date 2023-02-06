using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneMapper : MonoBehaviour
{
    [SerializeField]
    GameObject planeMarker;
    [SerializeField]
    GameObject floor;
    [SerializeField]
    int markerCount = 2;

    private void Start()
    {

    }

    public void CreateNewPlane(Vector3 marker1,Vector3 marker2)
    {
        float minY = Mathf.Min(marker2.y,marker1.y);
        marker1.y = minY;
        marker2.y = minY;
        ClearPlane();
        Instantiate(planeMarker, marker1, Quaternion.identity);
        Instantiate(planeMarker, marker2, Quaternion.identity);
        Instantiate(planeMarker, new Vector3(marker1.x,marker1.y,marker2.z), Quaternion.identity);
        Instantiate(planeMarker, new Vector3(marker2.x, marker2.y, marker1.z), Quaternion.identity);

        if (markerCount > 0) {
            Vector3 xStep = new Vector3((marker2.x - marker1.x) / (markerCount + 1),0, 0);
            Vector3 zStep = new Vector3(0, 0, (marker2.z - marker1.z) / (markerCount + 1));
            for (int i = 0;i < markerCount; i++)
            {
                Instantiate(planeMarker, marker1 + ((i + 1) * xStep), Quaternion.identity);
                Instantiate(planeMarker, marker1 + ((i + 1) * zStep), Quaternion.identity);
                Instantiate(planeMarker, marker2 - ((i + 1) * xStep), Quaternion.identity);
                Instantiate(planeMarker, marker2 - ((i + 1) * zStep), Quaternion.identity);
            }
        }
        GameObject newFloor = Instantiate(floor, (marker1 + marker2) * 0.5f, Quaternion.identity);
        newFloor.transform.localScale = new Vector3(Mathf.Abs(marker1.x - marker2.x), newFloor.transform.localScale.y, Mathf.Abs(marker1.z - marker2.z));
    }

    public void ClearPlane()
    {
        Destroy(GameObject.FindWithTag("Floor"));
        GameObject[] existingMarkers = GameObject.FindGameObjectsWithTag("PlaneMarker");
        for(int i = 0;i < existingMarkers.Length; i++)
        {
            Destroy(existingMarkers[i]);
        }

    }
}
