using System;
using System.Collections.Generic;

namespace mg.pummelz
{

    [System.Serializable]
    public class MGPumEncounter
    {
        


        public MGPumEncounter(string id, int level)
        {
            this.id = id;
            this.level = level;

            this.setups = new List<MGPumSetup>();
        }

        public string[] terrain;

        public string id;

        public int level;

        public string[] battlegrounds;

        public Dictionary<char, String> unitDictionary;
        public Dictionary<char, MGPumField.Terrain> terrainDictionary;

        public List<MGPumSetup> setups;


        public MGPumPlayerConfig getPlayerConfig()
        {
            MGPumPlayerConfig cfg = new MGPumPlayerConfig();
            return cfg;
        }

        public void addSetup(MGPumSetup setup)
        {
            this.setups.Add(setup);
        }

        internal MGPumEncounter deepCopy()
        {
            //keep what will not be changed
            MGPumEncounter copy = new MGPumEncounter(this.id, this.level);
            copy.terrain = this.terrain;
            copy.battlegrounds = this.battlegrounds;
            copy.unitDictionary = this.unitDictionary;
            copy.terrainDictionary = this.terrainDictionary;
            return copy;
        }

        
    }

        

}
