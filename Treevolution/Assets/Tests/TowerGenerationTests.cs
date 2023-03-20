using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TowerGenerationTests
{
    // A Test behaves as an ordinary method
    [Test]
    public void TowerGenerationPointAllocation()
    {
        for (int i = 0; i < 10; i++) // Attempts the following 10 times
        {
            int points = Random.Range(TowerData.numAttributes, 100);
            int numTowers = Random.Range(10, 100); // Generates a random number of towers to make, and a random number of points to allocate
            List<TowerData> towers = TowerGenerator.GenerateTowers(points, numTowers);
            Assert.AreEqual(numTowers, towers.Count); // Checks correct number of towers made
            foreach (TowerData tower in towers)
            {
                Assert.AreEqual(points, tower.Damage + tower.Range + tower.FireSpeed); // Checks correct point allocation
                Assert.GreaterOrEqual(tower.Damage, 1);
                Assert.GreaterOrEqual(tower.Range, 1);
                Assert.GreaterOrEqual(tower.FireSpeed, 1); // Checks no attribute is less than 1
            }
        }
    }

}