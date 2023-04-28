using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CreatorInfo
{
    public GameObject prefab;
    public int count;
}
public class Area : MonoBehaviour
{
    public bool isDebugDrawBoxLine = true;
    public bool generateOnStart = true;
    public List<CreatorInfo> allInfo;
    private GameObject floor;
    private List<GameObject> allObjects;
    private float height =0;
    [ContextMenu("GenerateObjectsByInfo")]
    public void GenerateObjectsByInfo()
    {
        ClearObjects();
        List<GameObject> notSetObjs = new List<GameObject>();
        Vector3 depthVec = GameProperties.TopLeftCorner - GameProperties.BottomLeftCorner;
        Vector3 widthVec = GameProperties.BottomRightCorner - GameProperties.BottomLeftCorner;
        for (int info = 0; info < allInfo.Count; info++)
        {
            for (int i = 0; i < allInfo[info].count; i++)
            {
                GameObject obj = Instantiate(allInfo[info].prefab);
                obj.transform.SetParent(floor.transform);
                obj.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                obj.transform.localPosition = Vector3.forward * Random.Range(-5.0f, 5.0f) + Vector3.right * Random.Range(-5.0f, 5.0f) + Vector3.up * height;
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
        height = GameProperties.FloorHeight;

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
            GenerateObjectsByInfo();
        }
    }
    
}
