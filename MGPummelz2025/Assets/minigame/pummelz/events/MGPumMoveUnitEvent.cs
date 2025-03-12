using UnityEngine;

namespace mg.pummelz
{
    [System.Serializable]
    public class MGPumMoveUnitEvent : MGPumGameEvent, MGPumUnitReferencable
    {
        public MGPumUnit movingUnit { get; set; }
        public MGPumField originField { get; set; }
        public MGPumField targetField { get; set; }

        public bool finalField { get; set; }


        public MGPumMoveUnitEvent(MGPumUnit movingUnit, MGPumField originField, MGPumField targetField, bool finalField = false) : base()
        {
            this.movingUnit = movingUnit;
            this.originField = originField;
            this.targetField = targetField;
            this.finalField = finalField;
        }

        public override void apply(MGPumGameState state)
        {
            movingUnit.field.unit = null;
            if(targetField.unit != null)
            {
                Debug.LogError(movingUnit + " moved to occupied field " + targetField + ", unit " + targetField.unit + " is overwritten!");
            }
            targetField.unit = movingUnit;
        }

        public override MGPumGameEvent deepCopy(MGPumGameState state)
        {
            MGPumUnit mu = (MGPumUnit)state.lookupOrCreate(this.movingUnit);
            MGPumField of = (MGPumField)state.lookupEntity(this.originField);
            MGPumField tf = (MGPumField)state.lookupEntity(this.targetField);

            MGPumGameEvent result = new MGPumMoveUnitEvent(mu, of, tf, finalField);
            this.copyToGameEvent(result);
            return result;
        }

        public MGPumUnit getReferencableUnit()
        {
            return movingUnit;
        }
    }
}
