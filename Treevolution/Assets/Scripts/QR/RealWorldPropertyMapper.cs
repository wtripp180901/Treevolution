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
        //REMEMBER THIS WHEN INTEGRATING !!!!!!!!!!!!!
        GetComponent<RoundTimer>().PauseTimer();
    }
    bool hasStarted = false;
    float time = 30f;
    private void Update()
    {
        time -= Time.deltaTime;
        if(time < 0 && !hasStarted)
        {
            //THIS TOO !!!!!!!!!!!!
            GetComponent<RoundTimer>().PauseTimer();
            GetComponent<PhaseTransition>().GoToGamePhase();
            hasStarted = true;
        }
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
