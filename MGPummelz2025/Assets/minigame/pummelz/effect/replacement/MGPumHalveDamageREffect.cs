﻿namespace mg.pummelz
{
    [System.Serializable]
    public class MGPumHalfDamageREffect : MGPumReplacementEffect
    {
        public override MGPumGameEvent replace(MGPumGameEvent eventToReplace, MGPumGameState state, MGPumEntity source)
        {
            MGPumDamageUnitEvent gEvent = (MGPumDamageUnitEvent)eventToReplace.deepCopy(state);
            if(gEvent.damage > 0)
            {
                gEvent.damage /= 2;
            }
            return gEvent;
        }

        internal override MGPumEffect deepCopy(MGPumGameState state)
        {
            return new MGPumHalfDamageREffect();
        }

    }
}
