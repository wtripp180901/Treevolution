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

    public void CreateNewPlane(Vector3 marker1,Vector3 marker2)
    {
        _minY = Mathf.Min(marker2.y,marker1.y);
        marker1.y = _minY;
        marker2.y = _minY;
        ClearPlane();

        Vector3 m1VerticalOpposite = new Vector3(marker1.x, marker1.y, marker2.z);
        Vector3 m2VerticalOpposite = new Vector3(marker2.x, marker2.y, marker1.z);
        if (marker1.x > marker2.x)
        {
            if(marker1.z > marker2.z)
            {
                tr = marker1;
                br = m1VerticalOpposite;
                tl = m2VerticalOpposite;
                bl = marker2;
            }
            else
            {
                tr = m1VerticalOpposite;
                br = marker1;
                tl = marker2;
                bl = m2VerticalOpposite;
            }
        }
        else
        {
            if (marker1.z > marker2.z)
            {
                tl = marker1;
                bl = m1VerticalOpposite;
                tr = m2VerticalOpposite;
                br = marker2;
            }
            else
            {
                tl = m1VerticalOpposite;
                bl = marker1;
                tr = marker2;
                br = m2VerticalOpposite;
            }
        }
        //Debug.DrawLine(tl,tl + Vector3.up,Color.green);
        Instantiate(planeMarker, tl, Quaternion.identity);
        Instantiate(planeMarker, tr, Quaternion.identity);
        Instantiate(planeMarker, bl, Quaternion.identity);
        Instantiate(planeMarker, br, Quaternion.identity);

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
        newFloor.transform.localScale = new Vector3(0.1f*Mathf.Abs(marker1.x - marker2.x), newFloor.transform.localScale.y, 0.1f*Mathf.Abs(marker1.z - marker2.z));
        Vector3 boardCentre = (marker1 + marker2) * 0.5f; // Centre of board
        GameObject.FindWithTag("InfoText").transform.position = boardCentre + new Vector3(0, 0.7f, 0);
        GameObject treeObject = GameObject.FindWithTag("Tree");
        if (treeObject == null)
        {
            Instantiate(treeModel, boardCentre, Quaternion.identity);
        }
        else
        {
            treeObject.transform.position = boardCentre;
        }

        GameObject.FindWithTag("NextRoundButton").transform.position = boardCentre + new Vector3(0, 0.9f, 0);
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
