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
    [SerializeField]
    GameObject startButton;

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
    private Pose _pose;
    public Pose pose { get { return _pose; } }
    private void Start()
    {
        //CreateNewPlane(new Vector3(0, 0, 0),new Vector3(1, 0, 1)); // Commented
    }

    public void CreateNewPlane(Pose marker)
    {
        marker.rotation = Quaternion.LookRotation(marker.up, marker.forward);
        marker.rotation = Quaternion.Euler(0, marker.rotation.eulerAngles.y, 0);
        _pose = marker;

        _minY = marker.position.y;
        ClearPlane();
       
        bl = marker.position;
        br = marker.position + _pose.forward * tableWidth * 0.01f;
        tl = marker.position - _pose.right * tableDepth * 0.01f;
        tr = marker.position + _pose.forward * tableWidth * 0.01f - _pose.right * tableDepth * 0.01f;

        tl.y = bl.y;
        br.y = bl.y;
        tr.y = bl.y;

   

        Debug.DrawLine(bl,bl + Vector3.up,Color.green,1000);
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

        Vector3 boardCentre = (bl + tr) * 0.5f; // Centre of board
        GameObject.FindWithTag("InfoText").transform.position = boardCentre + new Vector3(0, 0.7f, 0);
        GameObject newFloor = Instantiate(floor, (bl + tr) * 0.5f, _pose.rotation);
        Vector3 floorScale = new Vector3(0.1f * Vector3.Distance(bl, tl), newFloor.transform.localScale.y, 0.1f * Vector3.Distance(bl, br));
        newFloor.transform.localScale = floorScale;

        Debug.DrawLine(GameProperties.BottomLeftCorner, GameProperties.TopLeftCorner, Color.blue, 1000);
        Debug.DrawLine(GameProperties.BottomLeftCorner, GameProperties.BottomRightCorner, Color.blue, 1000);
        Debug.DrawLine(GameProperties.TopRightCorner, GameProperties.TopLeftCorner, Color.blue, 1000);
        Debug.DrawLine(GameProperties.TopRightCorner, GameProperties.BottomRightCorner, Color.blue, 1000);

        GameObject treeObject = GameObject.FindWithTag("Tree");
        if (treeObject == null)
        {
            Instantiate(treeModel, boardCentre, Quaternion.identity);
        }
        else
        {
            treeObject.transform.position = boardCentre;
        }

        startButton.transform.position = boardCentre + new Vector3(0, 0.8f, 0);
        startButton.SetActive(true);
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
