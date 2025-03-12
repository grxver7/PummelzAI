

using RelegatiaCCG.rccg.engine.state;

namespace mg.pummelz
{
    [System.Serializable]
    public class MGPumMarkerAbility : MGPumAbility
    {
        public enum Type
        {
            IgnoreTerrainMoving,
            JumpTerrainUnits
        }

        public Type type { get; set; }

        protected MGPumMarkerAbility()
        {
        }

        public MGPumMarkerAbility(Type type)
        {

            this.type = type;

        }

        protected void copyToMarkerAbility(MGPumMarkerAbility a)
        {
            base.copyToAbility(a);
            a.type = type;
        }

        public override MGPumAbility deepCopy()
        {
            MGPumMarkerAbility a = new MGPumMarkerAbility();
            this.copyToMarkerAbility(a);
            return a;
        }

        public override void setEffectIDs(IDManager idm)
        {

        }

        //TODO: does not support enabling conditions


    }
}
