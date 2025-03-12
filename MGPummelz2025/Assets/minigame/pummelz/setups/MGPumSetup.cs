using RelegatiaCCG.rccg.i18n;

namespace mg.pummelz
{

    [System.Serializable]
    public abstract class MGPumSetup : I18nizable
    {
        public abstract void apply(MGPumGameState state);

        public virtual string toI18nedString()
        {
            return this.GetType() + "";
        }
    }
}
