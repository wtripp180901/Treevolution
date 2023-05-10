using System.Collections.Generic;

namespace Towers
{
    //Holds a list of towers for a round
    public class TowerHolder
    {
        private List<TowerData> _towers;
        public TowerData[] towers { get { return _towers.ToArray(); } }

        public TowerHolder(List<TowerData> towers)
        {
            this._towers = towers;
        }
    }
}
