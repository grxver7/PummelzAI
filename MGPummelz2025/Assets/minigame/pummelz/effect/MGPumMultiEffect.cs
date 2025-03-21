﻿namespace mg.pummelz
{
    [System.Serializable]
    public abstract class MGPumMultiEffect : MGPumOneTimeEffect
    {

        internal abstract bool executeInternal(MGPumGameEvent parent, MGPumGameCommands gc, MGPumGameEventHandler handler, MGPumSelection selection, MGPumEntity source, MGPumGameEvent triggeringEvent, MGPumEffectExecutionState eestate);

        internal abstract MGPumEffect lookupSubEffect(MGPumEffect effect);

    }
}
