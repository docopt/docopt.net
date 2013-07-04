namespace NDocOpt
{
    /// <summary>
    ///     Marker/placeholder for [options] shortcut.
    /// </summary>
    public class OptionsShortcut : Optional
    {
        public OptionsShortcut() : base(new Pattern[0])
        {
        }
    }
}