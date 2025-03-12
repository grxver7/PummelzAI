namespace mg.pummelz
{
    [System.Serializable]
    public class MGPumCreateUnitEvent : MGPumZoneChangeEvent
    {
        private MGPumInstantiateUnitEvent ice;

        public override MGPumUnit unit { get {
                if(_unit == null)
                {
                    _unit = ice.instantiatedUnit;
                }
                return _unit; } set { _unit = value; } }

        public MGPumCreateUnitEvent(MGPumInstantiateUnitEvent ice, MGPumField field) : base(new MGPumNoneZoneURI(ice.ownerID), field)
        {
            this.unit = ice.instantiatedUnit;
            this.ice = ice;
        }

        public override void apply(MGPumGameState state)
        {
            this.unit = ice.instantiatedUnit;
            changeZone(state);
        }

        public MGPumUnit getCreatedUnit()
        {
            this.unit = ice.instantiatedUnit;
            return this.unit;
        }


        public override MGPumGameEvent deepCopy(MGPumGameState state)
        {
            MGPumInstantiateUnitEvent copiedICE = (MGPumInstantiateUnitEvent)state.lookupEvent(this.ice);
            MGPumField f = state.getField((MGPumField)(this.destination));

            MGPumCreateUnitEvent result = new MGPumCreateUnitEvent(copiedICE, f);
            this.copyToZoneChangeEvent(result, state);
            return result;
        }
    }
}
