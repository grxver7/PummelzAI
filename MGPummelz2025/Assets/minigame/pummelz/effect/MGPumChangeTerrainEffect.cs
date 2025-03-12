using RelegatiaCCG.rccg.engine.exceptions;
using System.Collections.Generic;

namespace mg.pummelz
{
    [System.Serializable]
    public class MGPumChangeTerrainEffect : MGPumSingleEffect
    {
        private MGPumSelector selector;
        private MGPumField.Terrain newTerrain;


        public MGPumChangeTerrainEffect(MGPumSelector selector, MGPumField.Terrain newTerrain)
        {
            this.selector = selector;
            this.newTerrain = newTerrain;
        }



        internal override void executeInternal(MGPumGameEvent parent, MGPumGameCommands gc, MGPumGameEventHandler handler, MGPumSelection selection, MGPumEntity source, MGPumGameEvent triggeringEvent, MGPumEffectExecutionState eestate)
        {
            List<MGPumEntity> selectedEntities = selection.getSelection(gc.state, source, triggeringEvent, eestate);
            foreach(MGPumEntity e in selectedEntities)
            {
                if(e is MGPumField)
                {
                    MGPumField f = (MGPumField)e;

                    gc.changeTerrain(parent, f, newTerrain);
                }
                else
                {
                    throw new GameException("Entity " + e + " is not a field.");
                }
                
            }

        }

        public override MGPumSelector getSelector(MGPumGameState state, MGPumEntity source)
        {
            return selector;
        }

        internal override MGPumEffect deepCopy(MGPumGameState state)
        {
            return new MGPumChangeTerrainEffect(this.selector.deepCopySelector(state), newTerrain);
        }
    }
}
