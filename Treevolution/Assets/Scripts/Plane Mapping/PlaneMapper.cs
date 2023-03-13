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

    float tableWidth = 240;
    float tableDepth = 160;

    public GameObject treeModel;


    private Vector3 tl;
    public Vector3 topLeft { get { return tl; } }
    private Vector3 tr;
    public Vector3 topRight { get { return tr; } }
    private Vector3 bl;
    public Vector3 bottomLeft { get { return bl; } }
    private Vector3 br;
    public Vector3 bottomRight { get { return br; } }
    private float _minY;
    public float floorHeight { get { return _minY; } }

    private void Start()
    {
        //CreateNewPlane(new Vector3(0, 0, 0),new Vector3(1, 0, 1)); // Commented
    }

    public void CreateNewPlane(Vector3 marker1, Pose orientation)
    {
        GameObject.FindWithTag("InfoText").transform.position = marker1 + new Vector3(0, 0.3f, 0);
        ClearPlane();

        bl = marker1;
        tl = marker1 + orientation.right * tableDepth * 0.01f;
        br = marker1 + orientation.up * tableWidth * 0.01f;
        tr = marker1 + orientation.right * tableDepth * 0.01f + orientation.up * tableWidth * 0.01f;

        tl.y = bl.y;
        br.y = bl.y;
        tr.y = bl.y;
        //Debug.DrawLine(tl,tl + Vector3.up,Color.green);
        Instantiate(planeMarker, tl, Quaternion.identity);
        Instantiate(planeMarker, tr, Quaternion.identity);
        Instantiate(planeMarker, bl, Quaternion.identity);
        Instantiate(planeMarker, br, Quaternion.identity);

        if (markerCount > 0) {
            Vector3 depthStep = (tl - bl) / (markerCount + 1);
            Vector3 widthStep = (br - bl) / (markerCount + 1);
            for (int i = 0;i < markerCount; i++)
            {
                Instantiate(planeMarker, bl + ((i + 1) * depthStep), Quaternion.identity);
                Instantiate(planeMarker, bl + ((i + 1) * widthStep), Quaternion.identity);
                Instantiate(planeMarker, tr - ((i + 1) * depthStep), Quaternion.identity);
                Instantiate(planeMarker, tr - ((i + 1) * widthStep), Quaternion.identity);
            }
        }
        GameObject newFloor = Instantiate(floor, (bl + tr) * 0.5f, Quaternion.LookRotation(-orientation.up, orientation.forward));
        newFloor.transform.localScale = new Vector3(0.1f*Vector3.Distance(bl, tl), newFloor.transform.localScale.y, 0.1f*Vector3.Distance(bl, br));
        Vector3 treeLocation = (bl + tr) * 0.5f; // Centre of board
        GameObject treeObject = GameObject.FindWithTag("Tree");
        if (treeObject == null)
        {
            Instantiate(treeModel, treeLocation, Quaternion.identity);
        }
        else
        {
            treeObject.transform.position = treeLocation;
        }

        GameObject.FindWithTag("NextRoundButton").transform.position = treeLocation + new Vector3(0, 0.5f, 0);
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
