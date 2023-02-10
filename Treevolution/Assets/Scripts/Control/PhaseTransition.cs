using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseTransition : MonoBehaviour
{
    public GameObject tree;
    public void GoToGamePhase()
    {
        GetComponent<RealWorldPropertyMapper>().MapProperties();
        GetComponent<EnemyManager>().StartSpawning();
        Vector3 vertical = GameProperties.TopLeftCorner - GameProperties.BottomLeftCorner;
        Vector3 horizontal = GameProperties.TopRightCorner - GameProperties.TopLeftCorner;
        Vector3 treeLocation = GameProperties.BottomRightCorner + (0.5f * vertical) - (0.1f * horizontal) + new Vector3(0,tree.GetComponent<Collider>().transform.localScale.y/2,0);
        Instantiate(tree, treeLocation, Quaternion.identity);
    }
}
