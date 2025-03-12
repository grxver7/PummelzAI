namespace mg.pummelz
{
    [System.Serializable]
    public class MGPumPlayerConfig
    {
        public MGPumPlayerConfig()
        {
        }


        public MGPumPlayerConfig deepCopy()
        {
            MGPumPlayerConfig copy = new MGPumPlayerConfig();

            return copy;
        }

    }
}
