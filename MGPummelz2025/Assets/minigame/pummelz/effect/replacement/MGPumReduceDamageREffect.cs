namespace mg.pummelz
{
    [System.Serializable]
    public class MGPumReduceDamageREffect : MGPumReplacementEffect
    {
        public override MGPumGameEvent replace(MGPumGameEvent eventToReplace, MGPumGameState state, MGPumEntity source)
        {
            MGPumDamageUnitEvent gEvent = (MGPumDamageUnitEvent)eventToReplace.deepCopy(state);
            if(gEvent.damage > 0)
            {
                gEvent.damage -= 1;
            }
            return gEvent;
        }

        internal override MGPumEffect deepCopy(MGPumGameState state)
        {
            return new MGPumReduceDamageREffect();
        }

    }
}
