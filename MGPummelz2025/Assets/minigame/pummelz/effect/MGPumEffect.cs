﻿using RelegatiaCCG.rccg.engine.state;

namespace mg.pummelz
{
    [System.Serializable]
    public abstract class MGPumEffect : MGPumEntity
    {

        public override MGPumZoneType getZone()
        {
            return MGPumZoneType.Destroyed;
        }

        internal abstract MGPumEffect deepCopy(MGPumGameState state);

        internal abstract void setEffectIDs(IDManager idm);

    }
}
