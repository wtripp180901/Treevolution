using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealWorldPropertyMapper : MonoBehaviour
{
    private PlaneMapper planeMapper;
    // Start is called before the first frame update
    void Start()
    {
        planeMapper = GetComponent<PlaneMapper>();
    }
    private void Update()
    {
    }

    public void MapProperties()
    {
        GameProperties.BottomLeftCorner = planeMapper.bottomLeft;
        GameProperties.BottomRightCorner = planeMapper.bottomRight;
        GameProperties.TopLeftCorner = planeMapper.topLeft;
        GameProperties.TopRightCorner = planeMapper.topRight;
        GameProperties.FloorHeight = planeMapper.floorHeight;
    }
}
