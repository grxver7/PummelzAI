namespace mg.pummelz
{
    [System.Serializable]
    public class MGPumOnTerrainCondition : MGPumSubCondition
    {
        private MGPumFilter filter;
        private MGPumField.Terrain terrain;
        public MGPumOnTerrainCondition(MGPumFilter selection, MGPumField.Terrain terrain)
        {
            this.filter = selection;
            this.terrain = terrain;
        }

        public override bool check(MGPumGameState state, MGPumEntity source)
        {
            bool result = true;
            foreach(MGPumEntity e in filter.apply(state, source))
            {
                if(!(
                    e is MGPumUnit 
                    && ((MGPumUnit)e).zone == MGPumZoneType.Battlegrounds
                    && ((MGPumUnit)e).field?.terrain == terrain))
                {
                    result = false;
                    break;
                }
            }

            return result;
        }


    }
}
