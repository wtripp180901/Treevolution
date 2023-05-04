using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CreatorInfo
{
    public GameObject prefab;
    public int count;
}
public class Grass : MonoBehaviour
{
    public bool isDebugDrawBoxLine = true;
    public bool generateOnStart = true;
    public List<CreatorInfo> allInfo;
    private GameObject floor;
    private List<GameObject> allObjects;
    private float height =0;
    [ContextMenu("GenerateObjectsByInfo")]
    public void GenerateGrass()
    {
        ClearObjects();
        height = GameProperties.FloorHeight;
        Vector3 depthVec = GameProperties.TopLeftCorner - GameProperties.BottomLeftCorner;
        Vector3 widthVec = GameProperties.BottomRightCorner - GameProperties.BottomLeftCorner;
        for (int info = 0; info < allInfo.Count; info++)
        {
            for (int i = 0; i < allInfo[info].count; i++)
            {
                GameObject obj = Instantiate(allInfo[info].prefab);
                //obj.transform.SetParent(floor.transform);
                obj.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                obj.transform.position = GameProperties.Centre + depthVec.normalized * Random.Range(-depthVec.magnitude/2, depthVec.magnitude/2) + widthVec.normalized * Random.Range(-widthVec.magnitude / 2, widthVec.magnitude / 2);
                obj.transform.localScale = new Vector3(5, obj.transform.localScale.y, 5);
                allObjects.Add(obj);
            }
        }
    }

    public void ClearObjects()
    {
        foreach (var item in allObjects)
        {
            Destroy(item);
        }
        allObjects.Clear();
    }

    private void Awake()
    {
        if (floor == null)
        {
            floor = gameObject;
        }
        allObjects = new List<GameObject>();
    }
    void Start()
    {
        if (generateOnStart)
        {
            GenerateGrass();
        }
    }

    private void OnDestroy()
    {
        ClearObjects();
    }

}
