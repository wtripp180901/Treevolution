using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Contains back-end data for the towers
public class TowerData
{
    private int damage { get; }
    private int range { get; }
    private int fireSpeed { get; }

    public static int numAttributes = 3; // number of attributes which determine tower behaviour
    public TowerData(int damage, int range, int fireSpeed)
    {
        this.damage = damage;
        this.range = range;
        this.fireSpeed = fireSpeed;
    }
    public TowerData()
    {
        this.damage = 0;
        this.range = 0;
        this.fireSpeed = 0;
    }
}
