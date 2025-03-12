namespace mg.pummelz
{
    [System.Serializable]
    public class MGPumChangeTerrainEvent : MGPumGameEvent
    {
        public MGPumField field { get; set; }
        public MGPumField.Terrain newTerrain { get; set; }

        public MGPumChangeTerrainEvent(MGPumField field, MGPumField.Terrain newTerrain) : base()
        {
            this.field = field;
            this.newTerrain = newTerrain;
        }

        public override void apply(MGPumGameState state)
        {
            field.terrain = newTerrain;
        }

        public override MGPumGameEvent deepCopy(MGPumGameState state)
        {
            MGPumField f = (MGPumField)state.lookupEntity(this.field);

            MGPumGameEvent result = new MGPumChangeTerrainEvent(f, newTerrain);
            this.copyToGameEvent(result);
            return result;
        }
    }
}
