using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Contains back-end data for the towers
public class TowerData
{
    public int Damage { get; private set; }
    public int Range { get; private set; }
    public int FireSpeed { get; private set; }

    public static int numAttributes = 3; // number of attributes which determine tower behaviour
    public TowerData(int damage, int range, int fireSpeed)
    {
        this.Damage = damage;
        this.Range = range;
        this.FireSpeed = fireSpeed;
    }
}
