namespace mg.pummelz
{
    public interface MGPumSelector
    {
       
        MGPumSelection getSelection();

        MGPumSelector deepCopySelector(MGPumGameState state);
    }
}
