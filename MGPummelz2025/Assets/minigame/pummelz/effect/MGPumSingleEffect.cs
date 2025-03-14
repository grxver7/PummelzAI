﻿using RelegatiaCCG.rccg.engine.state;

namespace mg.pummelz
{
    [System.Serializable]
    public abstract class MGPumSingleEffect : MGPumOneTimeEffect
    {

        internal abstract void executeInternal(MGPumGameEvent parent, MGPumGameCommands gc, MGPumGameEventHandler handler, MGPumSelection selection, MGPumEntity source, MGPumGameEvent triggeringEvent, MGPumEffectExecutionState eestate);

        public override MGPumSelector getSelector(MGPumGameState state, MGPumEntity source, MGPumEffectExecutionState eestate)
        {
            return getSelector(state, source);
        }

        internal override void setEffectIDs(IDManager idm)
        {
            this.id = idm.getNextID();
        }

    }
}
