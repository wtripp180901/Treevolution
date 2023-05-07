using System.Collections.Generic;
//using UnityEditor.PackageManager;
using UnityEngine;

namespace Towers
{
    public static class TowerGenerator
    {
        //Randomly generates a list of towers of length numberOfTowers, each with the points variable split randomly between their stats


        //Define some basic stats for TowerData (e.g damage, range, speed, etc) and implement GenerateTowers in TowerGenerator.cs.
        //It should generate a list towers, each of them with the given points randomly but fairly divided between each of the stats.
        //Define a setter method to give TowerScript a reference to its own unique TowerData

        /// <summary>
        /// Generates a number of TowerData objects each with an allocated number of points randomly distributed between each of their attributes.
        /// </summary>
        /// <param name="points">Number of points to allocate between attributes.</param>
        /// <param name="numberOfTowers">Number of towers to generate.</param>
        /// <returns></returns>
        public static List<TowerData> GenerateTowers(int points, int numberOfTowers)
        {
            if (points < TowerData.numAttributes)
            {
                return null; // Too few points to allocate to tower attributes
            }
            List<TowerData> towers = new List<TowerData>();
            points -= TowerData.numAttributes;
            for (int i = 0; i < numberOfTowers; i++)
            {
                float split1 = Random.Range(0.0f, 1.0f);
                float split2 = Random.Range(split1, 1.0f);

                int damage = 1 + (int)Mathf.Round(points * split1);
                int range = 1 + (int)Mathf.Round(points * (1 - split2));
                int speed = 3 + points - damage - range; // baseline of 1, +2 for the 2 subtracted
                towers.Add(new TowerData(damage, range, speed));
            }

            return towers;
        }
    }
}
