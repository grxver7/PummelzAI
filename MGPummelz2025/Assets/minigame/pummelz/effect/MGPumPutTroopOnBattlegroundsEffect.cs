using RelegatiaCCG.rccg.engine.exceptions;
using System;
using System.Collections.Generic;

namespace mg.pummelz
{
    [System.Serializable]
    public class MGPumPutUnitOnBattlegroundsEffect : MGPumSingleEffect
    {
        private MGPumSelector selectorUnit;

        private MGPumSelector selectorField;


        public MGPumPutUnitOnBattlegroundsEffect(MGPumSelector selectorUnit, MGPumSelector selectorField)
        {
            this.selectorUnit = selectorUnit;
            this.selectorField = selectorField;
        }



        internal override void executeInternal(MGPumGameEvent parent, MGPumGameCommands gc, MGPumGameEventHandler handler, MGPumSelection selection, MGPumEntity source, MGPumGameEvent triggeringEvent, MGPumEffectExecutionState eestate)
        {
            List<MGPumEntity> selectedEntities = selection.getSelection(gc.state, source, triggeringEvent, eestate);
            foreach(MGPumEntity e in selectedEntities)
            {
                if(e is MGPumUnit)
                {
                    MGPumUnit t = (MGPumUnit)e;

                    MGPumSelector fieldSelectorCopy = selectorField.deepCopySelector(gc.state);


                    //gc.putUnitOnBattlegrounds(parent, t, fieldSelectorCopy, source);
                    throw new NotImplementedException();
                }
                else
                {
                    throw new GameException("Entity " + e + " is not a unit.");
                }
                
            }

            
        }

        public override MGPumSelector getSelector(MGPumGameState state, MGPumEntity source)
        {
            return selectorUnit;
        }

        internal override MGPumEffect deepCopy(MGPumGameState state)
        {
            return new MGPumPutUnitOnBattlegroundsEffect(this.selectorUnit.deepCopySelector(state), this.selectorField.deepCopySelector(state));
        }
    }
}
