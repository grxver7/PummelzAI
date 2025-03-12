namespace mg.pummelz
{
    [System.Serializable]
    public abstract class MGPumCondition
    {
        public static MGPumStateCondition sc()
        {
            return new MGPumStateCondition();
        }
    }
}
