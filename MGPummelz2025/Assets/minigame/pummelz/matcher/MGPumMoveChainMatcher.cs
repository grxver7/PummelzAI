using UnityEngine;

namespace mg.pummelz
{
    [System.Serializable]
    public class MGPumMoveChainMatcher : MGPumFieldChainMatcher
    {

        public MGPumMoveChainMatcher(MGPumUnit unit) : base(unit)
        {
        }

        protected override bool isValidExtension(MGPumFieldChain chain, MGPumField first, MGPumField last)
        {
            //can only move over empty fields
            bool valid = true;
            int current = 0;
            foreach(MGPumField f in chain)
            {
                if(!isValidField(f, current, current + 1 == chain.getLength()))
                {
                    valid = false;
                }
                current++;
            }
            
            return valid;
        }

        private bool jumpPossible(int position, bool last)
        {
            if (startUnit.hasMarker(MGPumMarkerAbility.Type.JumpTerrainUnits))
            {
                
            }
            //we can only jump over inbetween fields
            return startUnit.hasMarker(MGPumMarkerAbility.Type.JumpTerrainUnits) && position < startUnit.currentSpeed && !last;
        }

        bool isValidField(MGPumField field, int position, bool last)
        {
            return (field.unit == null || field.unit == startUnit || jumpPossible(position, last))
                //check allowed terrains form movements
                && (startUnit.hasMarker(MGPumMarkerAbility.Type.IgnoreTerrainMoving) || jumpPossible(position, last) || field.terrain != MGPumField.Terrain.Water)
                && position <= startUnit.currentSpeed;
        }


        public override bool matchesChain(MGPumField field, MGPumFieldChain chain)
        {
            int position = chain.getLength();
            
            return (chain.last == null || chain.last.neighboring(field))
                && isValidField(field, position, position == startUnit.currentSpeed);
        }
    }



}

